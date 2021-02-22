using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelExtender.Common;
using LevelExtender.LEAPI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace LevelExtender.Framework.ItemBonus
{
    internal enum ItemBonusType
    {
        None = -2,

        // Vanilla Professions
        TillerCropsWorthMore = 1,
        TillerArtisanWorthMore = 4,
        TillerAgriculturistCropsGrowFaster = 5,
        RancherAnimalProductsWorthMore = 0,
        MinerMoreOre = 18,
        MinerBlacksmithBarsWorthMore = 20,
        MinerProspectorMoreCoal = 21,
        GeologistMoreGems = 19,
        GeologistExcavatorMoreGeodes = 22,
        GeologistGemologistGemsWorthMore = 23,
        GathererMoreForage = 13,
        ForesterTapperSyrupWorthMore = 15,
        FisherFishWorthMore = 6,
        FisherAnglerFishWorthMore = 8,

        // Custom Profession
        BetterQuality = -1
    }

    internal static class DefaultItemCategories
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
    }

    internal static class DefaultItems
    {
        public static readonly List<string> Any = new List<string>();
        public static readonly List<string> Coal = new List<string> { "Coal" };
        public static readonly List<string> Ores = new List<string> { "Copper Ore", "Gold Ore", "Iridium Ore", "Iron Ore" };
        public static readonly List<string> Bars = new List<string> { "Copper Bar", "Gold Bar", "Iridium Bar", "Iron Bar" };
        public static readonly List<string> Geodes = new List<string> { "Geode", "Frozen Geode", "Magma Geode", "Omni Geode" };
        public static readonly List<string> ExtraCrops = new List<string> { "Coffee Bean" };
    }

    class ItemBonusFromSkillValue
    {
        public double Chance { get; set; } = 1.0;
        public int Value { get; set; } = 0;
    }
    internal class ItemBonusFromSkill
    {
        public SkillType SkillType { get; set; }
        public ItemBonusType ItemBonusType { get; set; } = ItemBonusType.None;
        public List<int> ItemCategories { get; set; } = DefaultItemCategories.Any;
        public List<string> Items { get; set; } = DefaultItems.Any;
        public List<string> ExtraItems { get; set; } = DefaultItems.Any;

        public int MinLevel { get; set;} = 0;
        public List<ItemBonusFromSkillValue> Values { get; set; } = new List<ItemBonusFromSkillValue> { new ItemBonusFromSkillValue() };
        public double Chance
        {
            get
            {
                return (Values.Count > 0) ? Values[0].Chance : 1.0;
            }
            set
            {
                if (Values.Count == 0)
                {
                    Values.Add(new ItemBonusFromSkillValue { Chance = value });
                }
                else
                {
                    Values[0].Chance = value;
                }
                Values.Sort((a, b) => b.Chance.CompareTo(a.Chance));
            }
        }
        public int Value { 
           get {
                return (Values.Count > 0) ? Values[0].Value : 0;
           }
           set {
                if (Values.Count == 0)
                {
                    Values.Add(new ItemBonusFromSkillValue{ Value = value });
                }
                else {
                    Values[0].Value = value;
                }
           } 
        }

        public bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item)
        {
            var skill = skills.FirstOrDefault(s => s.Type == SkillType);
            var skillLevel = (skill != null) ? skill.Level : -1;

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyMoreDrops: skill: {skill.Name} {skillLevel}/{MinLevel}; item: {item.Name} ({item.Category}); {ItemBonusType} {Chance}; {ItemCategories.Count}, {Items.Count}");

            if (skillLevel >= MinLevel && (ItemCategories.Count == 0 || ItemCategories.Contains(item.Category) || ExtraItems.Contains(item.Name)) && (Items.Count == 0 || Items.Contains(item.Name)))
            {
                ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyMoreDrops: old stk: {item.Stack}");
                if (Chance > 0.0 && (Chance >= 1.0 || Game1.random.NextDouble() <= Chance))
                {
                    switch (ItemBonusType)
                    {
                        case ItemBonusType.GathererMoreForage:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    GathererMoreForage(skillLevel, item);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.MinerMoreOre:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    MinerMoreOre(skillLevel, item);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.MinerProspectorMoreCoal:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    MinerProspectorMoreCoal(skillLevel, item);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.GeologistMoreGems:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    GeologistMoreGems(skillLevel, item);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.GeologistExcavatorMoreGeodes:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    GeologistExcavatorMoreGeodes(skillLevel, item);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.None:
                            break;
                        case ItemBonusType.TillerCropsWorthMore:
                            break;
                        case ItemBonusType.TillerArtisanWorthMore:
                            break;
                        case ItemBonusType.TillerAgriculturistCropsGrowFaster:
                            break;
                        case ItemBonusType.RancherAnimalProductsWorthMore:
                            break;
                        case ItemBonusType.MinerBlacksmithBarsWorthMore:
                            break;
                        case ItemBonusType.GeologistGemologistGemsWorthMore:
                            break;
                        case ItemBonusType.ForesterTapperSyrupWorthMore:
                            break;
                        case ItemBonusType.FisherFishWorthMore:
                            break;
                        case ItemBonusType.FisherAnglerFishWorthMore:
                            break;
                        case ItemBonusType.BetterQuality:
                            break;
                    }
                }
            }

            return false;
        }


        public bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var skill = skills.FirstOrDefault(s => s.Type == SkillType);
            var skillLevel = (skill != null) ? skill.Level : -1;

            //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyWorthMore: skill: {skill.Name} {skillLevel}/{MinLevel}; item: {item.Name} ({item.Category}); {ItemBonusType} {Chance}; {ItemCategories.Count}, {Items.Count}");

            if (skillLevel >= MinLevel && (ItemCategories.Count == 0 || ItemCategories.Contains(item.Category) || ExtraItems.Contains(item.Name)) && (Items.Count == 0 || Items.Contains(item.Name)))
            {
                //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyWorthMore: old $ {newprice}");
                if (Chance > 0.0 && (Chance >= 1.0 || Game1.random.NextDouble() <= Chance))
                {
                    switch (ItemBonusType)
                    {
                        case ItemBonusType.TillerCropsWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    TillerCropsWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.TillerArtisanWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    TillerArtisanWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.RancherAnimalProductsWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    RancherAnimalProductsWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.ForesterTapperSyrupWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    ForesterTapperSyrupWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.MinerBlacksmithBarsWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    MinerBlacksmithBarsWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.GeologistGemologistGemsWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    GeologistGemologistGemsWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.FisherFishWorthMore:
                        case ItemBonusType.FisherAnglerFishWorthMore:
                            {
                                if (Game1.player.professions.Contains((int)ItemBonusType))
                                {
                                    FisherAnglerFishWorthMore(skillLevel, item, specificPlayerID, ref newprice);
                                    return true;
                                }
                                break;
                            }
                        case ItemBonusType.None:
                            break;
                        case ItemBonusType.TillerAgriculturistCropsGrowFaster:
                            break;
                        case ItemBonusType.MinerMoreOre:
                            break;
                        case ItemBonusType.MinerProspectorMoreCoal:
                            break;
                        case ItemBonusType.GeologistMoreGems:
                            break;
                        case ItemBonusType.GeologistExcavatorMoreGeodes:
                            break;
                        case ItemBonusType.GathererMoreForage:
                            break;
                        case ItemBonusType.BetterQuality:
                            break;
                    }
                }
            }

            return false;
        }

        public bool ApplyBetterQuality(IEnumerable<LESkill> skills, StardewValley.Object item)
        {
            var skill = skills.FirstOrDefault(s => s.Type == SkillType);
            var skillLevel = (skill != null) ? skill.Level : -1;

            ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyBetterQuality: skill: {skill.Name} {skillLevel}/{MinLevel}; item: {item.Name} ({item.Category}); {ItemBonusType} {Chance}; {ItemCategories.Count}, {Items.Count}");

            if (skillLevel >= MinLevel && (ItemCategories.Count == 0 || ItemCategories.Contains(item.Category) || ExtraItems.Contains(item.Name)) && (Items.Count == 0 || Items.Contains(item.Name)))
            {
                ModEntry.Logger.LogDebug($"ItemBonusFromSkill.ApplyBetterQuality: old Q {item.Quality}");
                var values = new List<ItemBonusFromSkillValue>(Values);
                values.Sort((a, b) => b.Value.CompareTo(a.Value));
                foreach (var value in values)
                {
                    if (value.Chance > 0.0 && (value.Chance >= 1.0 || Game1.random.NextDouble() <= value.Chance))
                    {
                        switch (ItemBonusType)
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

        private void TillerCropsWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void TillerArtisanWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void RancherAnimalProductsWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void GathererMoreForage(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Value;
        }
        private void ForesterTapperSyrupWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void MinerMoreOre(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Value;
        }
        private void MinerBlacksmithBarsWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void MinerProspectorMoreCoal(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Value;
        }
        private void GeologistGemologistGemsWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void FisherAnglerFishWorthMore(int skillLevel, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            sellToStorePrice(item, specificPlayerID, ref newprice, (100.0 + Value) / 100.0);
        }
        private void GeologistExcavatorMoreGeodes(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Value;
        }
        private void GeologistMoreGems(int skillLevel, StardewValley.Object item)
        {
            item.Stack += Value;
        }
        private void BetterQuality(int skillLevel, StardewValley.Object item, int value)
        {
            item.Quality = Math.Max(item.Quality, value);
        }

        public static void sellToStorePrice(StardewValley.Object item, long specificPlayerID, ref int newprice, double newpriceFactor)
        {
            newprice = (int)(newprice * newpriceFactor);
            //ModEntry.Logger.LogDebug($"ItemBonusFromSkill.sellToStorePrice: new $ {newprice}");
        }
    }
}

