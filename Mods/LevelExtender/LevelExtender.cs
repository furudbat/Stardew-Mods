using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LevelExtender.Common;
using LevelExtender.Framework;
using LevelExtender.Framework.Mods;
using LevelExtender.Framework.SkillTypes;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace LevelExtender
{
    class LevelExtender : ILevelExtender, LEModApi
    {
        public List<LESkill> Skills { get; private set; } = new List<LESkill>();
        public double MonstersSpawnRate { get; set; } = -1;
        public List<Monster> Monsters { get; private set; } = new List<Monster>();
        public bool NoMonsters
        {
            get { return Monsters.Count == 0; }
        }

        private static readonly int MAX_DOUBLE_ITEM_DROPS = 100;
        private static readonly List<int> REQUIRED_XP_TABLE = new List<int> { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

        public void RegisterMod(ISkillMod mod)
        {
            ModHandler.RegisterMod(mod);
        }

        public event EventHandler<EXPEventArgs> OnXPChanged
        {
            add => LEEvents.OnXPChanged += value;
            remove => LEEvents.OnXPChanged -= value;
        }

        public void Spawn_Rate(double osr)
        {
            MonstersSpawnRate = osr;
        }

        public int[] CurrentXP()
        {
            var ret = new List<int>();
            foreach (var skill in Skills) {
                ret.Add(skill.XP);
            }
            return ret.ToArray();
        }

        public int[] RequiredXP()
        {
            var ret = new List<int>();
            foreach (var skill in Skills)
            {
                ret.Add(skill.RequiredXPNextLevel);
            }
            return ret.ToArray();
        }
        public bool SetLevel(string name, int value)
        {
            var skill = Skills.FirstOrDefault(s => s.Name == name);
            if (skill != null) {
                skill.Level = value;
                return true;
            }
            return false;
        }
        public bool SetNeededXPFactor(string name, double value)
        {
            var skill = Skills.FirstOrDefault(s => s.Name == name);
            if (skill != null)
            {
                skill.NeededXPFactor = value;
                return true;
            }
            return false;
        }

        public ModConfig EditConfig(Action<ModConfig> func)
        {
            func(config);
            this.helper.WriteConfig(config);
            return config;
        }

        public LevelExtender(ModConfig config, IModHelper helper) {
            this.config = config;
            this.helper = helper;
            LEEvents.OnXPChanged += this.OnXPChangedEvent;
        }
        public void InitMod()
        {
            SetTimerShouldDrawXPBar(2000);
            InitSkills();
        }
        private void InitSkills()
        {
            /// TODO: use constants for item categories ?
            itemCategories[SkillType.Farming] = new List<int> { -16, -74, -75, -79, -80, -81 };
            itemCategories[SkillType.Fishing] = new List<int> { -4 };
            itemCategories[SkillType.Foraging] = itemCategories[SkillType.Farming];
            itemCategories[SkillType.Mining] = new List<int> { -2, -12, -15 };
            itemCategories[SkillType.Combat] = new List<int> { -28, -29, -95, -96, -98 };
            /// TODO: add item categories for custom skills, move itemCategories to Skill interface (?)

            Skills.Clear();
            foreach (var skill in Skill.AllSkills)
            {
                LESkill leSkill = new LESkill(skill, LEEvents, skill.GetSkillExperience(), 1.0, new List<int>(REQUIRED_XP_TABLE), itemCategories[skill.Type]);
                Skills.Add(leSkill);
            }

            Logger.LogVerbose("init: skills {skills}");
        }

        public void EndXPBar(SkillType skillType)
        {
            Logger.LogDebug($"EndXPBar: {skillType.Name}");
            XPBars = XPBars.Where(xpb => xpb.Skill.Skill.Type != skillType).ToList();

            if (XPBars.Count > 0)
            {
                XPBars[0].resetY();
            }
        }

        public void CleanUpMonters()
        {
            Monsters = Monsters.Where(monster => monster != null && monster.Health > 0 && monster.currentLocation != null).ToList();
        }

        public void CleanUpMod() {
            firstFade = false;
            lastMessage = "";
            lastDrawXPBarsTime = DateTime.Now;

            Monsters.Clear();
            XPBars.Clear();

            InitSkills();
        }

        public void CleanUpLastMessage() {
            lastMessage = "";
        }

        public void UpdateSkillXP() {
            foreach (var skill in Skill.DefaultSkills) {
                var leSkill = Skills.FirstOrDefault(s => s.Skill.Type == skill.Type);

                if (leSkill == null)
                {
                    Logger.LogError($"Skill {skill.Type} not registered properly for exp gain, please restart and/or report if no change.");
                    continue;
                }

                var skillXP = skill.GetSkillExperience();
                if (leSkill.XP != skillXP)
                {
                    Logger.LogDebug($"gain XP {leSkill.XP} -> {skillXP}");
                    leSkill.XP = skillXP;
                }
            }
        }

        public void SpawnRandomMonster() {
            var maxMonterSpawnTries = Math.Floor((float) Game1.player.currentLocation.Map.DisplayWidth / Game1.tileSize) * Math.Floor((float) Game1.player.currentLocation.Map.DisplayHeight / Game1.tileSize) + 1;
            Vector2 location = Game1.player.currentLocation.getRandomTile();
            for (int i = 0; i < maxMonterSpawnTries && !Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(location);i++)
            {
                location = Game1.player.currentLocation.getRandomTile();
            }

            int tier = rand.Next(0, 9);
            Monster monster = GenerateMonster(tier, GetMonster(tier, location * (float)Game1.tileSize));

            var characters = Game1.currentLocation.characters;
            characters.Add(monster);

            if (tier == 5) {
                Game1.chatBox.addMessage($"A boss has spawned in your current location!", Color.DarkRed);
            }

            Monsters.Add(monster);

            Logger.LogDebug($"spawn random Monster {monster.Name} {monster.Position}");
        }
        public double MonsterSpawnRate
        {
            get
            {
                if (Game1.player.combatLevel.Value == 0)
                {
                    return 0.0;
                }

                if (config.OverworldMonstersSpawnRate > 0.0)
                {
                    return config.OverworldMonstersSpawnRate;
                }
                else if (MonstersSpawnRate > 0.0)
                {
                    return MonstersSpawnRate;
                }

                if (Game1.isDarkOut() || Game1.isRaining)
                {
                    return (0.01 + (Game1.player.combatLevel.Value * 0.0001)) * 1.5;
                }

                return (0.01 + (Game1.player.combatLevel.Value * 0.0001));
            }
        }

        public void UpdateBobberBar() {
            if (Game1.activeClickableMenu is BobberBar && !firstFade)
            {
                int bobberBonus = 0;
                Tool tool = Game1.player.CurrentTool;
                bool isBeginnersRod = tool != null && tool is FishingRod && tool.UpgradeLevel == 1;

                var attachmentCorkBobber = tool.attachments.Where(attachment => attachment != null).FirstOrDefault(attachment => attachment.name == "Cork Bobber");
                if (attachmentCorkBobber != null)
                {
                    bobberBonus = 24;
                }

                if (Game1.player.FishingLevel > 99)
                    bobberBonus += 8;
                else if (Game1.player.FishingLevel > 74)
                    bobberBonus += 6;
                else if (Game1.player.FishingLevel > 49)
                    bobberBonus += 4;
                else if (Game1.player.FishingLevel > 24)
                    bobberBonus += 2;

                int bobberBarSize = 80;
                if (!(helper.ModRegistry.IsLoaded("DevinLematty.ExtremeFishingOverhaul")))
                {
                    if (isBeginnersRod)
                        bobberBarSize = 80 + (5 * 9);
                    else if (Game1.player.FishingLevel < 11)
                        bobberBarSize = 80 + bobberBonus + (int)(Game1.player.FishingLevel * 9);
                    else
                        bobberBarSize = 165 + bobberBonus + (int)(Game1.player.FishingLevel * (0.5 + (rand.NextDouble() / 2.0)));
                }
                else
                {
                    if (isBeginnersRod)
                        bobberBarSize = 80 + (5 * 7);
                    else if (Game1.player.FishingLevel < 11)
                        bobberBarSize = 80 + bobberBonus + (int)(Game1.player.FishingLevel * 7);
                    else if (Game1.player.FishingLevel > 10 && Game1.player.FishingLevel < 20)
                        bobberBarSize = 150 + bobberBonus + (int)(Game1.player.FishingLevel);
                    else
                        bobberBarSize = 170 + bobberBonus + (int)(Game1.player.FishingLevel * 0.8 * (0.5 + (rand.NextDouble() / 2.0)));
                }

                firstFade = true;
                helper.Reflection.GetField<int>(Game1.activeClickableMenu, "bobberBarHeight").SetValue(bobberBarSize);
                helper.Reflection.GetField<float>(Game1.activeClickableMenu, "bobberBarPos").SetValue((float)(568 - bobberBarSize));
            }
            else if (!(Game1.activeClickableMenu is BobberBar) && firstFade)
            {
                firstFade = false;
            }
            else if (Game1.activeClickableMenu is BobberBar && firstFade)
            {
                bool bobberInBar = helper.Reflection.GetField<bool>(Game1.activeClickableMenu, "bobberInBar").GetValue();
                if (!bobberInBar)
                {
                    float dist = helper.Reflection.GetField<float>(Game1.activeClickableMenu, "distanceFromCatching").GetValue();
                    helper.Reflection.GetField<float>(Game1.activeClickableMenu, "distanceFromCatching").SetValue(dist + ((float)(Game1.player.FishingLevel - 10) / 22000.0f));
                }
            }
        }

        public void SetTimerShouldDrawXPBar(int time)
        {
            shouldDrawXPBar = new Timer(time);
            shouldDrawXPBar.Elapsed += (sender, e) => shouldDrawXPBar.Enabled = false;
            shouldDrawXPBar.AutoReset = false;
            shouldDrawXPBar.Enabled = true;
        }

        public bool damageMonster_Prefix(
          Rectangle areaOfEffect,
          int minDamage,
          int maxDamage,
          bool isBomb,
          float knockBackModifier,
          int addedPrecision,
          float critChance,
          float critMultiplier,
          bool triggerMonsterInvincibleTimer,
          Farmer who)
        {
            var ret = true;

            if (config.MoreEXPByOneHitKills) {
                ret = OneHitKillsMoreXP(areaOfEffect, minDamage, maxDamage, isBomb, knockBackModifier, addedPrecision, critChance, critMultiplier, triggerMonsterInvincibleTimer, who) && ret;
            }

            // add more "damageMonster"-Handler here

            return ret;
        }

        private bool OneHitKillsMoreXP(
          Rectangle areaOfEffect,
          int minDamage,
          int maxDamage,
          bool isBomb,
          float knockBackModifier,
          int addedPrecision,
          float critChance,
          float critMultiplier,
          bool triggerMonsterInvincibleTimer,
          Farmer who) {
            GameLocation currentLocation = Game1.currentLocation;

            /// monster can spawn on farm
            //if (!currentLocation.IsFarm)
            //    return true;

            int damage = 0;
            for (int index = currentLocation.characters.Count - 1; index >= 0; --index)
            {
                Monster monster = (index < currentLocation.characters.Count && currentLocation.characters[index] is Monster) ? (Monster)currentLocation.characters[index] : null;
                if (monster != null && currentLocation.characters[index].IsMonster && monster.Health > 0 && monster.TakesDamageFromHitbox(areaOfEffect))
                {
                    if (!this.Monsters.Contains(monster))
                        continue;

                    if (monster.currentLocation == null)
                        monster.currentLocation = currentLocation;

                    if (!monster.IsInvisible && !monster.isInvincible() && (isBomb || helper.Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, monster, true) || helper.Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, monster, false)))
                    {
                        bool isDagger = !isBomb && who != null && (who.CurrentTool != null && who.CurrentTool is MeleeWeapon) && (who.CurrentTool as MeleeWeapon).type.Value == 1;
                        triggerMonsterInvincibleTimer = triggerMonsterInvincibleTimer || isDagger && MeleeWeapon.daggerHitsLeft > 1;

                        Rectangle boundingBox = monster.GetBoundingBox();
                        Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(boundingBox, who);
                        if (knockBackModifier > 0.0)
                            trajectory *= knockBackModifier;
                        else
                            trajectory = new Vector2(monster.xVelocity, monster.yVelocity);

                        if (monster.Slipperiness == -1)
                            trajectory = Vector2.Zero;

                        if (who != null && who.CurrentTool != null && monster.hitWithTool(who.CurrentTool))
                            return true;

                        if (who.professions.Contains(25))
                            critChance += critChance * 0.5f;

                        if (maxDamage >= 0)
                        {
                            int num = Game1.random.Next(minDamage, maxDamage + 1);
                            bool isCrit = (who != null && Game1.random.NextDouble() < critChance + who.LuckLevel * (critChance / 40.0));

                            int base_damage = Math.Max(1, (isCrit ? (int)(num * critMultiplier) : num) + (who != null ? who.attack * 3 : 0));
                            if (who != null && who.professions.Contains(24))
                                base_damage = (int)Math.Ceiling(base_damage * 1.10000002384186);
                            if (who != null && who.professions.Contains(26))
                                base_damage = (int)Math.Ceiling(base_damage * 1.14999997615814);
                            if (who != null & isCrit && who.professions.Contains(29))
                                base_damage = (int)(base_damage * 2.0);

                            if (who != null)
                            {
                                foreach (BaseEnchantment enchantment in who.enchantments)
                                    enchantment.OnCalculateDamage(monster, monster.currentLocation, who, ref base_damage);
                            }

                            damage = Math.Max(1, base_damage - monster.resilience.Value);
                            if (Game1.random.NextDouble() < monster.missChance.Value * addedPrecision)
                            {
                                return true;
                            }
                        }

                        // when monster one-hit-kill
                        if (monster.Health - damage <= 0)
                        {
                            Logger.LogDebug($"monster one-hit-kill with damage {damage}, gain xp: {monster.ExperienceGained}");
                            who.gainExperience(4, monster.ExperienceGained);
                        }
                    }
                }
            }

            return true;
        }

        public bool addItemToInventoryBool_Prefix(Item item, bool makeActiveObject)
        {
            var ret = true;

            if (config.DropExtraItems)
            {
                ret = DropExtraItems(item) && ret;
            }

            // add more "addItemToInventoryBool"-Handler here

            return ret;
        }

        private bool DropExtraItems(Item item)
        {
            if (item == null || item.HasBeenInInventory)
                return true;

            int item_category = item.Category;
            string message = "";

            //Logger.LogDebug($"DropExtraItems: {item.DisplayName} {item_category} {itemCategories}");

            int original_item_stack = item.Stack;
            foreach (var cat_entry in itemCategories)
            {
                if (message.Length > 0)
                    break;

                if (cat_entry.Value.Contains(item_category) && ShouldDoubleItem(cat_entry.Key))
                {
                    item.Stack += 1;

                    for (int i = 0; i < MAX_DOUBLE_ITEM_DROPS && ShouldDoubleItem(cat_entry.Key); i++)
                    {
                        item.Stack += 1;
                    }

                    if (config.drawExtraItemNotifications)
                    {
                        if (config.DropExtraItemMessageExtras)
                            message = $"Your {cat_entry.Key.Name} level allowed you to obtain {item.Stack - original_item_stack} extra {item.DisplayName}!";
                        else
                            message = $"Your {cat_entry.Key.Name} level allowed you to obtain extra {item.DisplayName}(s)!";
                    }
                }

                if (message.Length > 0)
                {
                    Logger.LogDebug($"'{message}'");
                }
            }

            if (message.Length > 0 && item.salePrice() >= config.minItemPriceForNotifications && message != lastMessage)
            {
                const float HUB_MESSAGE_TIME_LEFT = 3000;
                var messageColor = Color.DeepSkyBlue;

                if (config.DrawNotificationsAsHUDMessage)
                    Game1.addHUDMessage(new HUDMessage(message, messageColor, HUB_MESSAGE_TIME_LEFT, true));
                else
                    Game1.chatBox.addMessage(message, messageColor);

                lastMessage = message;
                SetTimerCooldownLastMessage(HUB_MESSAGE_TIME_LEFT * 2);
            }

            return true;
        }

        public void RemoveMonsters() 
        {
            foreach (GameLocation location in Game1.locations)
            {
                location.characters.Filter(character => !character.IsMonster);
            }
            Monsters.Clear();

            Logger.LogDebug($"remove Monsters");
        }
        public void RandomCropGrows()
        {
            Farm farm = Game1.getFarm();
            double growCompletelyChance = Game1.player.FarmingLevel * 0.0002;
            double addCropPhaseChance = Game1.player.FarmingLevel * 0.001;

            foreach (var key in farm.terrainFeatures.Keys)
            {
                var terrainFeatureHoeDirt = (farm.terrainFeatures[key] is HoeDirt) ? (HoeDirt) farm.terrainFeatures[key] : null;
                if (terrainFeatureHoeDirt != null && terrainFeatureHoeDirt.crop != null && rand.NextDouble() < growCompletelyChance)
                {
                    terrainFeatureHoeDirt.crop.growCompletely();

                    Logger.LogDebug($"randomCropGrows: growCompletely, {terrainFeatureHoeDirt.crop}");
                }
                else if (terrainFeatureHoeDirt != null && terrainFeatureHoeDirt.crop != null && rand.NextDouble() < addCropPhaseChance)
                {
                    terrainFeatureHoeDirt.crop.currentPhase.Value = Math.Min(terrainFeatureHoeDirt.crop.currentPhase.Value + 1, terrainFeatureHoeDirt.crop.phaseDays.Count - 1);

                    Logger.LogDebug($"randomCropGrows: add currentPhase {terrainFeatureHoeDirt.crop.currentPhase.Value}, {terrainFeatureHoeDirt.crop}");
                }
            }
        }

        public void DrawXPBars() {
            if (lastDrawXPBarsTime == null)
                lastDrawXPBarsTime = DateTime.Now;

            for (int i = 0; i < XPBars.Count; i++)
            {
                try
                {
                    if (i >= XPBars.Count || XPBars[i] == null)
                        continue;

                    int? top_ych = null;
                    if (i > 0 && XPBars.Count > 0)
                        top_ych = (int) XPBars[0].FadeY;

                    XPBars[i].DrawXPBar(lastDrawXPBarsTime, top_ych, i);
                    //Logger.LogDebug($"DrawXPBar {i} {XPBars[i].Skill.Skill.Type.Name}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Non-Serious draw violation: DrawXPBar {i}/{XPBars.Count}  {ex.Message}");
                }
            }

            lastDrawXPBarsTime = DateTime.Now;
        }

        private bool ShouldDoubleItem(SkillType skillType)
        {
            double drate = (skillType == SkillType.Farming || skillType == SkillType.Foraging) ? 0.002 / 2.0 : 0.002;
            var skill = Skills.FirstOrDefault(s => s.Skill.Type == skillType);
            return skill != null && rand.NextDouble() <= (skill.Level * drate);
        }
        private void OnXPChangedEvent(object sender, EXPEventArgs e)
        {
            var bar = XPBars.FirstOrDefault(b => b.Skill.Skill.Type == e.SkillType);
            var skill = Skills.FirstOrDefault(s => s.Skill.Type == e.SkillType);

            Logger.LogDebug($"OnXPChangedEvent: {skill.XP}/{skill.RequiredXPNextLevel}");

            if (skill == null || skill.ChangedXP < 0 || skill.ChangedXP > 100001 || shouldDrawXPBar.Enabled)
                return;

            if (bar != null)
            {
                bar.FadeTimer.Stop();
                bar.FadeTimer.Start();
                bar.FadeTime = DateTime.Now;
                double val = -bar.FadeY;

                SetBarsYchVals(val);
                SortByTime();
            }
            else
            {
                XPBars.Add(new XPBar(this, skill));
                Logger.LogDebug($"OnXPChangedEvent: add XPBars {skill.Skill.Type.Name}");
            }
        }
        private void SortByTime()
        {
            XPBars = XPBars.OrderBy(o => o.FadeTime).ToList();
        }
        private void SetBarsYchVals(double val)
        {
            foreach (var bar in XPBars)
            {
                bar.FadeY = val;
            }
        }

        private static Monster GenerateMonster(int tier, Monster monster)
        {
            if (tier == 8)
            {
                tier = 5;
                monster.resilience.Value += 20;
                monster.Slipperiness += rand.Next(10) + 5;
                monster.coinsToDrop.Value = rand.Next(10) * 50;
                monster.startGlowing(new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)), true, 1.0f);
                monster.Health *= 1 + (rand.Next(Game1.player.CombatLevel / 2, Game1.player.CombatLevel));

                var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");

                monster.objectsToDrop.Add(rand.Next(data.Count));
                monster.displayName += " (BOSS)";
                monster.Scale = monster.Scale * (float)(1 + (rand.NextDouble() * Game1.player.CombatLevel / 25.0));
            }
            else
            {
                tier = 1;
            }

            monster.DamageToFarmer = (int)(monster.DamageToFarmer / 1.5) + (int)(Game1.player.combatLevel.Value / 3);
            monster.Health *= 1 + (Game1.player.CombatLevel / 4);
            monster.focusedOnFarmers = true;
            monster.wildernessFarmMonster = true;
            monster.Speed += rand.Next((int)Math.Round((Game1.player.combatLevel.Value / 10.0)));
            monster.resilience.Set(monster.resilience.Value + (Game1.player.combatLevel.Value / 10));
            monster.ExperienceGained += (int)(monster.Health / 100.0) + ((10 + (Game1.player.combatLevel.Value * 2)) * tier);

            return monster;
        }
        private static Monster GetMonster(int tier, Vector2 position)
        {
            switch (tier)
            {
                case 0:
                    return new DustSpirit(position);
                case 1:
                    return new Grub(position);
                case 2:
                    return new Skeleton(position);
                case 3:
                    return new RockCrab(position);
                case 4:
                    return new Ghost(position);
                case 5:
                    return new GreenSlime(position);
                case 6:
                    return new RockGolem(position);
                case 7:
                    return new ShadowBrute(position);
                case 8:
                    int subTier = rand.Next(1, 6);
                    if (subTier == 1)
                        return new RockCrab(position, "Iridium Crab");
                    else if (subTier == 2)
                        return new Ghost(position, "Carbon Ghost");
                    else if (subTier == 3)
                        return new LavaCrab(position);
                    else if (subTier == 4)
                        return new GreenSlime(position, Math.Max(Game1.player.CombatLevel * 5, 50));
                    else if (subTier == 5)
                        return new BigSlime(position, Math.Max(Game1.player.CombatLevel * 5, 50));
                    else
                        return new Mummy(position);
            }

            return new Monster();
        }

        private void SetTimerCooldownLastMessage(float time)
        {
            cooldownLastMessage = new Timer(time);
            cooldownLastMessage.Elapsed += (sender, e) => lastMessage = "";
            cooldownLastMessage.AutoReset = false;
            cooldownLastMessage.Enabled = true;
        }

        private static Random rand = new Random(Guid.NewGuid().GetHashCode());

        private readonly ModConfig config;
        private IModHelper helper;
        private LEEvents LEEvents = new LEEvents();
        private List<XPBar> XPBars = new List<XPBar>();
        private Timer shouldDrawXPBar;
        private Timer cooldownLastMessage;
        private DateTime lastDrawXPBarsTime;
        private bool firstFade = false;
        private string lastMessage = "";
        private Dictionary<SkillType, List<int>> itemCategories = new Dictionary<SkillType, List<int>>();
    }

}

