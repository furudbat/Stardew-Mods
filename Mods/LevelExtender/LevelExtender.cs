﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LevelExtender.Framework;
using LevelExtender.Framework.ItemBonus;
using LevelExtender.Logging;
using LevelExtender.UIElements;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace LevelExtender
{
    internal class LevelExtender : ILevelExtender
    {
        public ModConfig Config { get; set; }
        public Logger Logger { get; private set; }
        public List<BaseSkill> Skills { get; private set; }
        public double MonstersSpawnRate { get; set; }
        public List<Monster> Monsters { get; private set; }
        public bool NoMonsters
        {
            get { return Monsters.Count == 0; }
        }

        public static readonly int MAX_DOUBLE_ITEM_DROPS = 20;
        public static readonly List<int> REQUIRED_XP_TABLE = new List<int> { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };
        public static readonly int BOBBER_BASEBAR_SIZE = 82;

        public LevelExtender(ModConfig config, IModHelper helper, IMonitor logMonitor)
        {
            this.Config = config;
            this.helper = helper;
            Logger = new Logger(config, logMonitor);
            Skills = new List<BaseSkill>();
            Monsters = new List<Monster>();
            MonstersSpawnRate = -1;
            ResetXPBar();
        }

        public event EventHandler<LEXPEventArgs> OnXPChanged
        {
            add => LEEvents.OnXPChanged += value;
            remove => LEEvents.OnXPChanged -= value;
        }

        public void Spawn_Rate(double osr)
        {
            MonstersSpawnRate = osr;
        }

        public int[] currentXP()
        {
            var ret = new List<int>();
            foreach (var skill in Skills)
            {
                ret.Add(skill.GetExperienceFor(Game1.player));
            }
            return ret.ToArray();
        }

        public int[] requiredXP()
        {
            var ret = new List<int>();
            foreach (var skill in Skills)
            {
                ret.Add(skill.GetRequiredXPNextLevel(Game1.player));
            }
            return ret.ToArray();
        }
        public IEnumerable<ISkill> GetSkills()
        {
            return Skills.Cast<ISkill>();
        }
        public int GetSkillCurrentXP(string skillId)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            return (skill != null) ? skill.GetExperienceFor(Game1.player) : -1;
        }
        public int GetSkillRequiredXP(string skillId)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            return (skill != null) ? skill.GetRequiredXPNextLevel(Game1.player) : -1;
        }
        public int GetSkillLevel(string skillId)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            return (skill != null) ? skill.GetSkillLevel(Game1.player) : -1;
        }
        public int GetSkillMaxLevel(string skillId)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            return (skill != null) ? skill.MaxLevel : -1;
        }
        public int GetXPRequiredToLevel(string skillId, int level)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            return (skill != null && level >= 0 && level < skill.XPTable.Count && level <= skill.MaxLevel) ? skill.XPTable[level] : (skill != null) ? skill.MaxXP : -1;
        }

        public bool SetNeededXPFactor(string skillId, double value)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            if (skill != null)
            {
                skill.NeededXPFactor = value;
                return true;
            }
            return false;
        }

        public ModConfig EditConfig(Action<ModConfig> func)
        {
            func(Config);
            XPBar.ToggleShowExperienceBar(Config.DrawXPBars);
            XPBar.ToggleShowExperienceGain(Config.DrawXPGain);
            this.helper.WriteConfig(Config);
            return Config;
        }

        public void InitMod()
        {
            InitSkills();
            ResetXPBar();
        }
        public void ResetXPBar()
        {
            XPBar = new ExtendedExperienceBar(this.helper, this);
            XPBar.ToggleShowExperienceBar(Config.DrawXPBars);
            XPBar.ToggleShowExperienceGain(Config.DrawXPGain);
        }
        private void InitSkills()
        {
            extraItemCategories.Clear();
            Skills.Clear();
            foreach (var skill in SkillsList.DefaultSkills)
            {
                double xp_factor = (skill.Name == SkillsList.DefaultSkillNames.Fishing)? 0.5 : 1.0;
                if (skill.Name == SkillsList.DefaultSkillNames.Combat) {
                    xp_factor = 0.75;
                }
                var leSkill = new LEVanillaSkill(Game1.player, skill, LEEvents, xp_factor, new List<int>(REQUIRED_XP_TABLE));
                Skills.Add(leSkill);
                extraItemCategories.Add(skill.Name, new List<int>(skill.ExtraItemCategories()));

                ModEntry.Logger.LogDebug($"InitSkills: vanilla skill: {leSkill.Id} {leSkill.GetName()}, lvl.  {leSkill.Level}, xp: {leSkill.XP}, {leSkill}");
            }

            foreach (var scSkillName in SpaceCore.Skills.GetSkillList())
            {
                var skill = SpaceCore.Skills.GetSkill(scSkillName);
                if (skill != null)
                {
                    var scSkill = new SCSkill(skill);
                    Skills.Add(scSkill);

                    ModEntry.Logger.LogDebug($"InitSkills: sc skill: {scSkill.Id} {scSkill.GetName()}, lvl.  {scSkill.GetSkillLevel(Game1.player)}, xp: {scSkill.GetExperienceFor(Game1.player)}, {scSkill}");
                }

            }

            UpdateExtraItemCategorySkillNames();

            ModEntry.Logger.LogDebug($"InitSkills: skills {Skills.Count}");
        }

        private void UpdateExtraItemCategorySkillNames()
        {
            Dictionary<int, HashSet<string>> skillNames = new Dictionary<int, HashSet<string>>();
            foreach (var cat_entry in extraItemCategories)
            {
                var skillName = cat_entry.Key;
                var itemCategories = cat_entry.Value;
                foreach (var item_category in itemCategories)
                {
                    if (!skillNames.ContainsKey(item_category))
                    {
                        skillNames.Add(item_category, new HashSet<string>());
                    }
                    skillNames[item_category].Add(skillName);
                }
            }

            extraItemCategorySkillNames.Clear();
            foreach (var cat_entry in extraItemCategories)
            {
                var skillName = cat_entry.Key;
                var itemCategories = cat_entry.Value;
                foreach (var itemCategory in itemCategories)
                {
                    if (skillNames.ContainsKey(itemCategory))
                    {
                        skillName = string.Join("/", skillNames[itemCategory]);
                    }

                    if (!extraItemCategorySkillNames.ContainsKey(itemCategory))
                    {
                        extraItemCategorySkillNames.Add(itemCategory, skillName);
                    }
                    else
                    {
                        extraItemCategorySkillNames[itemCategory] = skillName;
                    }
                }
            }
        }

        public void CleanUpMonters()
        {
            Monsters = Monsters.Where(monster => monster != null && monster.Health > 0 && monster.currentLocation != null).ToList();
        }
        public void CleanUpMod()
        {
            firstFade = false;
            lastMessage = "";

            Monsters.Clear();
            XPBar.Dispose();

            Skills.Clear();
        }

        public void CleanUpLastMessage()
        {
            lastMessage = "";
        }

        public void UpdateVanillaSkillsXP()
        {
            foreach (var leSkill in Skills.Where(s => s is LEVanillaSkill).Cast<LEVanillaSkill>())
            {
                var skillXP = leSkill.Skill.GetSkillExperience();
                if (leSkill.XP != skillXP)
                {
                    Logger.LogDebug($"gain XP {leSkill.GetExperienceFor(Game1.player)} -> {skillXP}");
                    leSkill.XP = skillXP;
                }
            }
        }

        public void SpawnRandomMonster()
        {
            var maxMonterSpawnTries = Math.Floor((float)Game1.player.currentLocation.Map.DisplayWidth / Game1.tileSize) * Math.Floor((float)Game1.player.currentLocation.Map.DisplayHeight / Game1.tileSize) + 1;
            Vector2 location = Game1.player.currentLocation.getRandomTile();
            for (int i = 0; i < maxMonterSpawnTries && !Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(location); i++)
            {
                location = Game1.player.currentLocation.getRandomTile();
            }
            if (!Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(location))
            {
                return;
            }

            int tier = Game1.random.Next(0, 9);
            Monster monster = GenerateMonster(tier, GetMonster(tier, location * (float)Game1.tileSize));

            var characters = Game1.currentLocation.characters;
            characters.Add(monster);

            if (tier == 5)
            {
                Game1.chatBox.addMessage($"A boss has spawned in your current location!", Color.DarkRed);
            }

            Monsters.Add(monster);

            Logger.LogDebug($"spawn random Monster {monster.Name} {monster.Position}");
        }
        public double MonsterSpawnRate
        {
            get
            {
                var skill = Skills.FirstOrDefault(s => s.Id == SkillsList.DefaultSkillNames.Combat);
                var combatLevel = (skill != null) ? skill.GetSkillLevel(Game1.player) : Game1.player.CombatLevel;

                if (combatLevel == 0)
                {
                    return 0.0;
                }

                if (Config.OverworldMonstersSpawnRate > 0.0)
                {
                    return Config.OverworldMonstersSpawnRate;
                }
                else if (MonstersSpawnRate > 0.0)
                {
                    return MonstersSpawnRate;
                }

                if (Game1.isDarkOut() || Game1.isRaining)
                {
                    return (0.01 + (combatLevel * 0.0001)) * 1.5;
                }

                return (0.01 + (combatLevel * 0.0001));
            }
        }

        public List<LEVanillaSkill> VanillaSkills => Skills.Where(s => s is LEVanillaSkill).Cast<LEVanillaSkill>().ToList();

        public void UpdateBobberBar()
        {
            var skill = Skills.FirstOrDefault(s => s.Id == SkillsList.DefaultSkillNames.Fishing);
            var fishingLevel = (skill != null) ? skill.GetSkillLevel(Game1.player) : Game1.player.FishingLevel;

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

                if (fishingLevel > 99)
                    bobberBonus += 8;
                else if (fishingLevel > 74)
                    bobberBonus += 6;
                else if (fishingLevel > 49)
                    bobberBonus += 4;
                else if (fishingLevel > 24)
                    bobberBonus += 2;

                int bobberBarSize = BOBBER_BASEBAR_SIZE;
                if (!(helper.ModRegistry.IsLoaded("DevinLematty.ExtremeFishingOverhaul")))
                {
                    if (isBeginnersRod)
                        bobberBarSize = BOBBER_BASEBAR_SIZE + (5 * 9);
                    else if (fishingLevel < 11)
                        bobberBarSize = BOBBER_BASEBAR_SIZE + bobberBonus + (int)(fishingLevel * 9);
                    else
                        bobberBarSize = (2 * BOBBER_BASEBAR_SIZE + 5) + bobberBonus + (int)(fishingLevel * 0.5);
                }
                else
                {
                    if (isBeginnersRod)
                        bobberBarSize = BOBBER_BASEBAR_SIZE + (5 * 7);
                    else if (fishingLevel < 11)
                        bobberBarSize = BOBBER_BASEBAR_SIZE + bobberBonus + (int)(fishingLevel * 7);
                    else if (fishingLevel > 10 && fishingLevel < 20)
                        bobberBarSize = BOBBER_BASEBAR_SIZE + (3 * BOBBER_BASEBAR_SIZE / 4) + bobberBonus + (int)(fishingLevel);
                    else
                        bobberBarSize = (2 * BOBBER_BASEBAR_SIZE) + bobberBonus + (int)(fishingLevel * 0.8);
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
                    helper.Reflection.GetField<float>(Game1.activeClickableMenu, "distanceFromCatching").SetValue(dist + ((float)(fishingLevel - 10) / 22000.0f));
                }
            }
        }

        public bool damageMonster_Prefix(
          GameLocation currentLocation,
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
            if (Config.MoreEXPByOneHitKills)
            {
                ret = OneHitKillsMoreXP(currentLocation, areaOfEffect, minDamage, maxDamage, isBomb, knockBackModifier, addedPrecision, critChance, critMultiplier, triggerMonsterInvincibleTimer, who) && ret;
            }

            // add more "damageMonster"-Handler here

            return ret;
        }

        private bool OneHitKillsMoreXP(
          GameLocation currentLocation,
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

            if (item is StardewValley.Object obj)
            {
                if (Config.BetterItemQuality)
                {
                    ret = ItemsBetterQuality(obj) && ret;
                }
                if (Config.DropExtraItemsByProfession)
                {
                    ret = ItemsMoreDrops(obj) && ret;
                }
                if (Config.DropExtraItemsByLevel)
                {
                    ret = DropExtraItems(obj) && ret;
                }
            }

            // add more "addItemToInventoryBool"-Handler here

            return ret;
        }

        public void plant_Postfix(HoeDirt hoeDirt, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location)
        {
            Logger.LogDebug($"plant_Postfix: {index} {isFertilizer} {hoeDirt.crop}");
            if (hoeDirt == null || hoeDirt.crop == null)
                return;

            if (Config.CropsGrow == CropsGrowOption.LinearByLevel)
            {
                ExtraCropGrow(hoeDirt);
            }
        }

        private void ExtraCropGrow(HoeDirt hoeDirt)
        {
            if (hoeDirt.crop == null || hoeDirt.crop.dead.Value)
                return;

            foreach (var itemBonuses in ItemBonuses.RegisteredItemBonuses)
            {
                ItemBonuses.ApplyCropGrow(Game1.player, itemBonuses.Where(ib => ib.BonusType == ItemBonusType.CropsGrowFaster).ToList(), Skills, hoeDirt);
            }
        }

        private bool ItemsMoreDrops(StardewValley.Object obj)
        {
            if (obj == null || obj.HasBeenInInventory)
                return true;

            foreach (var itemBonuses in ItemBonuses.RegisteredItemBonuses)
            {
                ItemBonuses.ApplyMoreDrops(Game1.player, itemBonuses.Where(ib => ib.BonusType == ItemBonusType.MoreDrops).ToList(), Skills, obj);
            }

            return true;
        }
        private bool ItemsBetterQuality(StardewValley.Object obj)
        {
            if (obj == null || obj.HasBeenInInventory)
                return true;

            foreach (var itemBonuses in ItemBonuses.RegisteredItemBonuses)
            {
                ItemBonuses.ApplyBetterQuality(Game1.player, itemBonuses.Where(ib => ib.BonusType == ItemBonusType.BetterQuality).ToList(), Skills, obj);
            }

            return true;
        }
        private void ItemsWorthMore(StardewValley.Object obj, long specificPlayerID, ref int newprice)
        {
            if (obj == null)
                return;

            foreach (var itemBonuses in ItemBonuses.RegisteredItemBonuses)
            {
                ItemBonuses.ApplyWorthMore(Game1.player, itemBonuses.Where(ib => ib.BonusType == ItemBonusType.WorthMore).ToList(), Skills, obj, specificPlayerID, ref newprice);
            }
        }
        private bool DropExtraItems(StardewValley.Object item)
        {
            if (item == null || item.HasBeenInInventory)
                return true;

            int itemCategory = item.Category;
            string message = "";

            //Logger.LogDebug($"DropExtraItems: {item.DisplayName} {item_category} {itemCategories}");

            int originalItemStack = item.Stack;
            foreach (var cat_entry in extraItemCategories)
            {
                if (message.Length > 0)
                    break;

                var skillName = cat_entry.Key;
                if (!cat_entry.Value.Contains(itemCategory))
                {
                    continue;
                }
                for (int i = 0; i < MAX_DOUBLE_ITEM_DROPS && ShouldDoubleItem(skillName, item); i++)
                {
                    item.Stack += 1;
                    message = ShowExtraItemDropMessage(skillName, originalItemStack, item);
                }
            }

            return true;
        }

        private string ShowExtraItemDropMessage(string skillName, int originalItemStack, Item item)
        {
            int itemCategory = item.Category;
            if (extraItemCategorySkillNames.ContainsKey(itemCategory))
            {
                skillName = extraItemCategorySkillNames[itemCategory];
            }

            var message = "";
            if (Config.DrawExtraItemNotifications != DrawExtraItemNotificationsOption.Disable)
            {
                if (Config.DrawExtraItemNotifications == DrawExtraItemNotificationsOption.EnableWithAmount)
                    message = I18n.ExtraItemMessageWithAmount(skillName: skillName, extraItemAmount: item.Stack - originalItemStack, itemName: item.DisplayName);
                else
                    message = I18n.ExtraItemMessage(skillName: skillName, itemName: item.DisplayName);
            }

            if (message.Length > 0 && item.salePrice() >= Config.MinItemPriceForNotifications && message != lastMessage)
            {
                const float HUB_MESSAGE_TIME_LEFT = 1000;
                var messageColor = Color.DeepSkyBlue;

                if (Config.DrawNotificationsAsHUDMessage)
                    Game1.addHUDMessage(new HUDMessage(message, item.Stack - originalItemStack, true, messageColor, item));
                else
                    Game1.chatBox.addMessage(message, messageColor);

                lastMessage = message;
                SetTimerCooldownLastMessage(HUB_MESSAGE_TIME_LEFT * 4);
            }

            return message;
        }
        public void sellToStorePrice_Postfix(StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            if (Config.BetterItemQuality || Config.DropExtraItemsByProfession)
            {
                ItemsWorthMore(item, specificPlayerID, ref newprice);
            }

            // add more "sellToStorePrice"-Handler here
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
            var skill = Skills.FirstOrDefault(s => s.Id == SkillsList.DefaultSkillNames.Farming);
            var farmingLevel = (skill != null) ? skill.GetSkillLevel(Game1.player) : Game1.player.FarmingLevel;

            Farm farm = Game1.getFarm();
            double growCompletelyChance = farmingLevel * 0.0002;
            double addCropPhaseChance = farmingLevel * 0.001;

            foreach (var key in farm.terrainFeatures.Keys)
            {
                HoeDirt terrainFeatureHoeDirt = (farm.terrainFeatures[key] is HoeDirt) ? (HoeDirt)farm.terrainFeatures[key] : null;
                if (terrainFeatureHoeDirt != null && terrainFeatureHoeDirt.crop != null && Game1.random.NextDouble() < growCompletelyChance)
                {
                    terrainFeatureHoeDirt.crop.growCompletely();

                    Logger.LogDebug($"randomCropGrows: growCompletely, {terrainFeatureHoeDirt.crop}");
                }
                else if (terrainFeatureHoeDirt != null && terrainFeatureHoeDirt.crop != null && Game1.random.NextDouble() < addCropPhaseChance)
                {
                    terrainFeatureHoeDirt.crop.currentPhase.Value = Math.Min(terrainFeatureHoeDirt.crop.currentPhase.Value + 1, terrainFeatureHoeDirt.crop.phaseDays.Count - 1);

                    Logger.LogDebug($"randomCropGrows: add currentPhase {terrainFeatureHoeDirt.crop.currentPhase.Value}, {terrainFeatureHoeDirt.crop}");
                }
            }
        }

        private bool ShouldDoubleItem(string skillId, StardewValley.Object item)
        {
            double drop_rate = (skillId == SkillsList.DefaultSkillNames.Farming || skillId == SkillsList.DefaultSkillNames.Foraging) ? 0.002 / 2.0 : 0.002;
            var skill = Skills.FirstOrDefault(s => s.Id == skillId);
            if (item.Stack >= 2)
            {
                drop_rate = drop_rate / (3.0 * item.Stack / 4.0);
            }
            if (item.Quality >= 1)
            {
                drop_rate /= (item.Quality + 1);
            }
            return skill != null && item.Stack > 0 && Game1.random.NextDouble() <= ((skill.GetSkillLevel(Game1.player) * drop_rate) + ((double)Game1.player.DailyLuck / 1200.0 + 9.9999997473787516E-05));
        }

        private Monster GenerateMonster(int tier, Monster monster)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == SkillsList.DefaultSkillNames.Combat);
            var combatLevel = (skill != null) ? skill.GetSkillLevel(Game1.player) : Game1.player.CombatLevel;

            if (tier == 8)
            {
                tier = 5;
                monster.resilience.Value += 20;
                monster.Slipperiness += Game1.random.Next(10) + 5;
                monster.coinsToDrop.Value = Game1.random.Next(10) * 50;
                monster.startGlowing(new Color(Game1.random.Next(0, 255), Game1.random.Next(0, 255), Game1.random.Next(0, 255)), true, 1.0f);
                monster.Health *= 1 + (Game1.random.Next(combatLevel / 2, combatLevel));

                var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");

                monster.objectsToDrop.Add(Game1.random.Next(data.Count));
                monster.displayName += " (BOSS)";
                monster.Scale *= (float)(1 + (Game1.random.NextDouble() * combatLevel / 25.0));
            }
            else
            {
                tier = 1;
            }

            monster.DamageToFarmer = (int)(monster.DamageToFarmer / 1.5) + (int)(combatLevel / 3);
            monster.Health *= 1 + (combatLevel / 4);
            monster.focusedOnFarmers = true;
            monster.wildernessFarmMonster = true;
            monster.Speed += Game1.random.Next((int)Math.Round((combatLevel / 10.0)));
            monster.resilience.Set(monster.resilience.Value + (combatLevel / 10));
            monster.ExperienceGained += (int)(monster.Health / 100.0) + ((10 + (combatLevel * 2)) * tier);

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
                    int subTier = Game1.random.Next(1, 6);
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

        public void SetItemCategories(string skillName, IEnumerable<int> itemCategories)
        {
            var skill = Skills.FirstOrDefault(s => s.Id == skillName);
            if (skill != null) {
                skill.ItemCategories = itemCategories.ToList();
            }
        }

        public void RegisterItemBonuses(IEnumerable<ItemBonusBySkillData> values)
        {
            ItemBonuses.RegisterItemBonus(values);
        }

        public bool SetVanillaLevel(string skillId, int value)
        {
            var skill = Skills.Where(s => s is LEVanillaSkill).Cast<LEVanillaSkill>().FirstOrDefault(s => s.Id == skillId);
            if (skill != null) {
                skill.Level = value;
                return true;
            }
            return false;
        }

        public bool SetVanillaXP(string skillId, int value)
        {
            var skill = Skills.Where(s => s is LEVanillaSkill).Cast<LEVanillaSkill>().FirstOrDefault(s => s.Id == skillId);
            if (skill != null)
            {
                skill.XP = value;
                return true;
            }
            return false;
        }

        private readonly IModHelper helper;
        private readonly LEEvents LEEvents = new LEEvents();
        private ExtendedExperienceBar XPBar;
        private Timer cooldownLastMessage;
        private bool firstFade = false;
        private string lastMessage = "";
        private readonly Dictionary<string, List<int>> extraItemCategories = new Dictionary<string, List<int>>();
        private readonly Dictionary<int, string> extraItemCategorySkillNames = new Dictionary<int, string>();
    }

}

