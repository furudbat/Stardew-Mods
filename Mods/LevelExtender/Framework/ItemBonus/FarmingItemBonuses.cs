using System.Collections.Generic;
using LevelExtender.Common;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Framework.ItemBonus
{
    internal class TillerItemBonus : ItemBonusFromSkill
    {
        public TillerItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerCropsWorthMore;
            ItemCategories = DefaultItemCategories.Crops;
        }
    }
    internal class TillerArtisanItemBonus : TillerItemBonus
    {
        public TillerArtisanItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerArtisanWorthMore;
            ItemCategories = DefaultItemCategories.Artisan;
        }
    }
    internal class TillerAgriculturistItemBonus : TillerItemBonus
    {
        public TillerAgriculturistItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerAgriculturistCropsGrowFaster;
        }
    }
    internal class RangerItemBonus : ItemBonusFromSkill
    {
        public RangerItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.RancherAnimalProductsWorthMore;
            ItemCategories = DefaultItemCategories.AnimalProducts;
        }
    }
    internal class BetterQualityCropsItemBonus : ItemBonusFromSkill
    {
        public BetterQualityCropsItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.BetterQuality;
            ItemCategories = DefaultItemCategories.Crops;
            ExtraItems = DefaultItems.ExtraCrops;
        }
    }

    internal static class FarmingItemBonuses
    {
        public static List<TillerItemBonus> TillerItemBonuses => new List<TillerItemBonus>
        {
            new TillerItemBonus
            {
                MinLevel = 10,
                Value = 15
            },
            new TillerItemBonus
            {
                MinLevel = 20,
                Value = 20
            },
            new TillerItemBonus
            {
                MinLevel = 30,
                Value = 30
            },
            new TillerItemBonus
            {
                MinLevel = 50,
                Value = 40
            },
            new TillerItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new TillerItemBonus
            {
                MinLevel = 100,
                Value = 80
            },
        };
        public static List<TillerArtisanItemBonus> ArsianItemBonuses => new List<TillerArtisanItemBonus>
        {
            new TillerArtisanItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 50,
                Value = 60
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 70,
                Value = 85
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 100,
                Value = 110
            },
        };
        public static List<RangerItemBonus> RangerItemBonuses => new List<RangerItemBonus>
        {
            new RangerItemBonus
            {
                MinLevel = 10,
                Value = 40
            },
            new RangerItemBonus
            {
                MinLevel = 20,
                Value = 45
            },
            new RangerItemBonus
            {
                MinLevel = 30,
                Value = 50
            },
            new RangerItemBonus
            {
                MinLevel = 50,
                Value = 55
            },
            new RangerItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new RangerItemBonus
            {
                MinLevel = 100,
                Value = 80
            },
        };
        public static List<BetterQualityCropsItemBonus> BetterQualityCropsItemBonuses => new List<BetterQualityCropsItemBonus>
        {
            new BetterQualityCropsItemBonus
            {
                MinLevel = 15,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 0.38, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.23, Value = ItemQuality.Gold },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 20,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 0.44, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.25, Value = ItemQuality.Gold },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 30,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 0.60, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.33, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.01, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 40,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 0.75, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.45, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.05, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 50,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 0.89, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.55, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.09, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 60,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 1.0, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.65, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.14, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 70,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 1.0, Value = ItemQuality.Silver },
                    new ItemBonusFromSkillValue { Chance = 0.75, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.18, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 80,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.25, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 90,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.38, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 100,
                Values = {
                    new ItemBonusFromSkillValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusFromSkillValue { Chance = 0.51, Value = ItemQuality.Irudium },
                }
            },
        };

        public static List<TillerAgriculturistItemBonus> TillerAgriculturistItemBonuses => new List<TillerAgriculturistItemBonus>
        {
            new TillerAgriculturistItemBonus
            {
                MinLevel = 20,
                Value = 5
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 30,
                Value = 9
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 40,
                Value = 12
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 50,
                Value = 15
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 60,
                Value = 18
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 70,
                Value = 21
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 80,
                Value = 24
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 90,
                Value = 27
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 100,
                Value = 30
            },
        };

        public static bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item)
        {
            var ret = false;
            ret = ItemBonuses.ApplyMoreDrops(TillerItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(ArsianItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(RangerItemBonuses, skills, item) || ret;
            return ret;
        }
        public static bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var ret = false;
            ret = ItemBonuses.ApplyWorthMore(TillerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(ArsianItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(RangerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            return ret;
        }
        public static bool ApplyBetterQuality(IEnumerable<LESkill> skills, StardewValley.Object item)
        {
            var ret = false;
            ret = ItemBonuses.ApplyBetterQuality(BetterQualityCropsItemBonuses, skills, item) || ret;
            return ret;
        }

        internal static bool ApplyCropGrow(List<LESkill> skills, HoeDirt hoeDirt)
        {
            var ret = false;
            ret = ItemBonuses.ApplyCropGrow(TillerAgriculturistItemBonuses, skills, hoeDirt) || ret;
            return ret;
        }
    }
}
