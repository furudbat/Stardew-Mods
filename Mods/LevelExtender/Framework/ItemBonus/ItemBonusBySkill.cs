using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Framework.ItemBonus
{
    public static class ItemBonusProfession
    {
        public const int None = -1;

        // Vanilla Professions
        public const int TillerCropsWorthMore = 1;
        public const int TillerArtisanWorthMore = 4;
        public const int TillerAgriculturistCropsGrowFaster = 5;
        public const int RancherAnimalProductsWorthMore = 0;
        public const int MinerMoreOre = 18;
        public const int MinerBlacksmithBarsWorthMore = 20;
        public const int MinerProspectorMoreCoal = 21;
        public const int GeologistMoreGems = 19;
        public const int GeologistExcavatorMoreGeodes = 22;
        public const int GeologistGemologistGemsWorthMore = 23;
        public const int GathererMoreForage = 13;
        public const int ForesterTapperSyrupWorthMore = 15;
        public const int FisherFishWorthMore = 6;
        public const int FisherAnglerFishWorthMore = 8;

        // item bonus non-Profession
        public const int BetterQuality = -2;
    }

    public static class ItemBonusType
    {
        public const int Unknown = -99;
        public const int None = -1;
        public const int WorthMore = 0;
        public const int CropsGrowFaster = 1;
        public const int MoreDrops = 2;
        public const int BetterQuality = -2;
    }

    public static class DefaultItemCategories
    {
        public static readonly List<int> Any = new List<int>();
        public static readonly List<int> Farming = new List<int> { StardewValley.Object.SeedsCategory, StardewValley.Object.VegetableCategory, StardewValley.Object.FruitsCategory, StardewValley.Object.flowersCategory };
        public static readonly List<int> Fishing = new List<int> { StardewValley.Object.FishCategory };
        public static readonly List<int> Foraging = new List<int> { StardewValley.Object.buildingResources, StardewValley.Object.SeedsCategory, StardewValley.Object.FruitsCategory, StardewValley.Object.flowersCategory, StardewValley.Object.GreensCategory };
        public static readonly List<int> Mining = new List<int> { StardewValley.Object.GemCategory, StardewValley.Object.mineralsCategory, StardewValley.Object.metalResources };
        public static readonly List<int> Combat = new List<int> { StardewValley.Object.monsterLootCategory, StardewValley.Object.equipmentCategory, StardewValley.Object.hatCategory, StardewValley.Object.ringCategory, StardewValley.Object.weaponCategory };

        public static readonly List<int> Crops = new List<int> { StardewValley.Object.VegetableCategory, StardewValley.Object.FruitsCategory, StardewValley.Object.flowersCategory };
        public static readonly List<int> Artisan = new List<int> { StardewValley.Object.artisanGoodsCategory, StardewValley.Object.syrupCategory };
        public static readonly List<int> OresOrBars = new List<int> { StardewValley.Object.metalResources };
        public static readonly List<int> AnimalProducts = new List<int> { StardewValley.Object.EggCategory, StardewValley.Object.EggCategory, StardewValley.Object.MilkCategory, StardewValley.Object.meatCategory, StardewValley.Object.sellAtPierresAndMarnies };
        public static readonly List<int> Gems = new List<int> { StardewValley.Object.GemCategory };
        public static readonly List<int> GemsOrMinerals = new List<int> { StardewValley.Object.GemCategory, StardewValley.Object.mineralsCategory };
        public static readonly List<int> Syrup = new List<int> { StardewValley.Object.syrupCategory };
        public static readonly List<int> Fish = new List<int> { StardewValley.Object.FishCategory };
        public static readonly List<int> GreensForaging = new List<int> { StardewValley.Object.GreensCategory };
    }

    public static class DefaultItems
    {
        public static readonly List<string> Any = new List<string>();
        public static readonly List<string> Coal = new List<string> { "Coal" };
        public static readonly List<string> Ores = new List<string> { "Copper Ore", "Gold Ore", "Iridium Ore", "Iron Ore" };
        public static readonly List<string> Bars = new List<string> { "Copper Bar", "Gold Bar", "Iridium Bar", "Iron Bar" };
        public static readonly List<string> Geodes = new List<string> { "Geode", "Frozen Geode", "Magma Geode", "Omni Geode" };
        public static readonly List<string> ExtraCrops = new List<string> { "Coffee Bean" };
    }

    class ItemBonusBySkill 
    {
        public ItemBonusBySkillData Bonus { get; set; } = new ItemBonusBySkillData();

        public ItemBonusBySkill(ItemBonusBySkillData bonus)
        {
            Bonus = bonus;
        }
        public ItemBonusBySkill() {
            Bonus.Profession = ItemBonusProfession.None;
            Bonus.ItemCategories = DefaultItemCategories.Any;
            Bonus.Items = DefaultItems.Any;
            Bonus.ExtraItems = DefaultItems.Any;
            Bonus.MinLevel = 0;
        }
        public int BonusType
        {
            get
            {
                if (Bonus.BonusType != null) {
                    return Bonus.BonusType.Value;
                }

                switch (Bonus.Profession)
                {
                    case ItemBonusProfession.None:
                        return ItemBonusType.None;
                    case ItemBonusProfession.TillerCropsWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.TillerArtisanWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.TillerAgriculturistCropsGrowFaster:
                        return ItemBonusType.CropsGrowFaster;
                    case ItemBonusProfession.RancherAnimalProductsWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.MinerMoreOre:
                        return ItemBonusType.MoreDrops;
                    case ItemBonusProfession.MinerBlacksmithBarsWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.MinerProspectorMoreCoal:
                        return ItemBonusType.MoreDrops;
                    case ItemBonusProfession.GeologistMoreGems:
                        return ItemBonusType.MoreDrops;
                    case ItemBonusProfession.GeologistExcavatorMoreGeodes:
                        return ItemBonusType.MoreDrops;
                    case ItemBonusProfession.GeologistGemologistGemsWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.GathererMoreForage:
                        return ItemBonusType.MoreDrops;
                    case ItemBonusProfession.ForesterTapperSyrupWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.FisherFishWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.FisherAnglerFishWorthMore:
                        return ItemBonusType.WorthMore;
                    case ItemBonusProfession.BetterQuality:
                        return ItemBonusType.BetterQuality;
                }

                return ItemBonusType.Unknown;
            }
        }

        public bool ApplyMoreDrops(Farmer farmer, IEnumerable<BaseSkill> skills, StardewValley.Object item)
        {
            var skill = skills.FirstOrDefault(s => s.Id == Bonus.SkillId);
            var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyMoreDrops: skill: {skill.Id} {skillLevel}/{Bonus.MinLevel}; item: {item.Name} ({item.Category}); prof. {Bonus.Profession} {Bonus.Chance}; {Bonus.ItemCategories.Count}, {Bonus.Items.Count}");

            if (skillLevel >= Bonus.MinLevel && (Bonus.ItemCategories.Count == 0 || Bonus.ItemCategories.Contains(item.Category) || Bonus.ExtraItems.Contains(item.Name)) && (Bonus.Items.Count == 0 || Bonus.Items.Contains(item.Name)))
            {
                ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyMoreDrops: old stk: {item.Stack}");
                if (Bonus.Chance > 0.0 && (Bonus.Chance >= 1.0 || Game1.random.NextDouble() <= Bonus.Chance))
                {
                    switch (BonusType)
                    {
                        case ItemBonusType.MoreDrops:
                            {
                                if (Game1.player.professions.Contains(Bonus.Profession))
                                {
                                    MoreDrops(skillLevel, item);
                                    return true;
                                }
                            };
                            break;
                    }
                }
            }

            return false;
        }

        public bool ApplyCropGrow(Farmer farmer, List<BaseSkill> skills, HoeDirt hoeDirt)
        {
            var skill = skills.FirstOrDefault(s => s.Id == Bonus.SkillId);
            var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyCropGrow: skill: {skill.Id} {skillLevel}/{Bonus.MinLevel}; prof. {Bonus.Profession} {Bonus.Chance}; {Bonus.ItemCategories.Count}, {Bonus.Items.Count}");

            if (skillLevel >= Bonus.MinLevel)
            {
                ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyCropGrow: crop phase: {hoeDirt?.crop?.currentPhase}");
                if (Bonus.Chance > 0.0 && (Bonus.Chance >= 1.0 || Game1.random.NextDouble() <= Bonus.Chance))
                {
                    switch (BonusType)
                    {
                        case ItemBonusType.CropsGrowFaster:
                            {
                                if (Game1.player.professions.Contains(Bonus.Profession))
                                {
                                    CropsGrowFaster(skillLevel, hoeDirt);
                                    return true;
                                }
                            }
                            break;
                    }
                }
            }

            return false;
        }


        public bool ApplyWorthMore(Farmer farmer, IEnumerable<BaseSkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var skill = skills.FirstOrDefault(s => s.Id == Bonus.SkillId);
            var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;

            //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyWorthMore: skill: {skill.Name} {skillLevel}/{MinLevel}; item: {item.Name} ({item.Category}); {ItemBonusType} {Chance}; {ItemCategories.Count}, {Items.Count}");

            if (skillLevel >= Bonus.MinLevel && (Bonus.ItemCategories.Count == 0 || Bonus.ItemCategories.Contains(item.Category) || Bonus.ExtraItems.Contains(item.Name)) && (Bonus.Items.Count == 0 || Bonus.Items.Contains(item.Name)))
            {
                //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyWorthMore: old $ {newprice}");
                if (Bonus.Chance > 0.0 && (Bonus.Chance >= 1.0 || Game1.random.NextDouble() <= Bonus.Chance))
                {
                    switch (BonusType)
                    {
                        case ItemBonusType.WorthMore:
                            {
                                if (Game1.player.professions.Contains(Bonus.Profession))
                                {
                                    WorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                            };
                            break;
                    }
                }
            }

            return false;
        }

        public bool ApplyBetterQuality(Farmer farmer, IEnumerable<BaseSkill> skills, StardewValley.Object item)
        {
            var skill = skills.FirstOrDefault(s => s.Id == Bonus.SkillId);
            var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyBetterQuality: skill: {skill.Id} {skillLevel}/{Bonus.MinLevel}; item: {item.Name} ({item.Category}); {Bonus.Profession} {Bonus.Chance}; {Bonus.ItemCategories.Count}, {Bonus.Items.Count}");

            if (skillLevel >= Bonus.MinLevel && (Bonus.ItemCategories.Count == 0 || Bonus.ItemCategories.Contains(item.Category) || Bonus.ExtraItems.Contains(item.Name)) && (Bonus.Items.Count == 0 || Bonus.Items.Contains(item.Name)))
            {
                ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyBetterQuality: old Q {item.Quality}");
                var values = new List<ItemBonusValue>(Bonus.Values);
                values.Sort((a, b) => b.Value.CompareTo(a.Value));
                foreach (var value in values)
                {
                    if (value.Chance > 0.0 && (value.Chance >= 1.0 || Game1.random.NextDouble() <= value.Chance))
                    {
                        switch (BonusType)
                        {
                            case ItemBonusType.BetterQuality:
                                BetterQuality(skillLevel, item, value.Value);
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private void WorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Bonus.Value) / 100.0);
        }
        private void MoreDrops(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Bonus.Value;
        }
        private void BetterQuality(int skillLevel, StardewValley.Object item, int value)
        {
            item.Quality = Math.Max(item.Quality, value);
        }
        private void sellToStorePrice(StardewValley.Object item, long specificPlayerID, ref int newprice, double newpriceFactor)
        {
            newprice = (int)(newprice * newpriceFactor);
            //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.sellToStorePrice: new $ {newprice}");
        }
        private void CropsGrowFaster(int skillLevel, HoeDirt hoeDirt)
        {
            Crop crop = hoeDirt.crop;
            if (crop ==  null) {
                return;
            }


            int daysOfGrow = 0;
            int phases = crop.phaseDays.Count;
            if (crop.fullyGrown.Value && crop.dayOfCurrentPhase.Value > 0)
            {
                daysOfGrow = crop.dayOfCurrentPhase.Value;
            }
            else
            {
                for (int i = 0; i < crop.phaseDays.Count - 1; ++i)
                {
                    if (hoeDirt.crop.currentPhase.Value == i)
                        daysOfGrow -= crop.dayOfCurrentPhase.Value;

                    if (hoeDirt.crop.currentPhase.Value <= i)
                        daysOfGrow += crop.phaseDays[i];
                }
            }

            if (crop.phaseDays.Count == 0)
            {
                return;
            }

            int subPhases = (int)(daysOfGrow * Bonus.Value / 100.0);
            if (crop.fullyGrown.Value && crop.dayOfCurrentPhase.Value > 0)
            {
                crop.dayOfCurrentPhase.Value -= Math.Max(1, subPhases);
            }
            else
            {
                for (int i = 0; i < crop.phaseDays.Count - 1 && subPhases > 0; ++i)
                {
                    if (i == crop.phaseDays.Count-2)
                    {
                        crop.phaseDays[i] -= Math.Max(1, subPhases);
                    } else if (crop.phaseDays[i] > 1)
                    {
                        crop.phaseDays[i]--;
                        subPhases--;
                    }
                }
            }

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.CropsGrowFaster: new phases {crop.phaseDays[0]}...{crop.phaseDays[crop.phaseDays.Count-1]}");
        }
    }
}

