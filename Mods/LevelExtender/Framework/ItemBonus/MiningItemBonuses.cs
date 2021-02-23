using System.Collections.Generic;
using LevelExtender.Common;

namespace LevelExtender.Framework.ItemBonus
{
    internal class MinerItemBonus : ItemBonusFromSkill
    {
        public MinerItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.MinerMoreOre;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Ores;
        }
    }
    internal class MinerBlacksmithItemBonus : MinerItemBonus
    {
        public MinerBlacksmithItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.MinerBlacksmithBarsWorthMore;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Bars;
        }
    }
    internal class MinerProspectorItemBonus : MinerItemBonus
    {
        public MinerProspectorItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.MinerProspectorMoreCoal;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Coal;
        }
    }
    internal class GeologistItemBonus : ItemBonusFromSkill
    {
        public GeologistItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.GeologistMoreGems;
            ItemCategories = DefaultItemCategories.Gems;
            Items = DefaultItems.Any;
        }
    }
    internal class GeologistExcavatorItemBonus : GeologistItemBonus
    {
        public GeologistExcavatorItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.GeologistExcavatorMoreGeodes;
            ItemCategories = DefaultItemCategories.Any;
            Items = DefaultItems.Geodes;
        }
    }
    internal class GeologistGemologistItemBonus : GeologistItemBonus
    {
        public GeologistGemologistItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Mining;
            ItemBonusType = ItemBonusType.GeologistGemologistGemsWorthMore;
            ItemCategories = DefaultItemCategories.GemsOrMinerals;
            Items = DefaultItems.Any;
        }
    }

    internal static class MiningItemBonuses
    {
        public static List<MinerItemBonus> MinerItemBonuses => new List<MinerItemBonus>
        {
            new MinerItemBonus
            {
                MinLevel = 10,
                Value = 1
            },
            new MinerItemBonus
            {
                MinLevel = 20,
                Value = 2
            },
            new MinerItemBonus
            {
                MinLevel = 30,
                Value = 3
            },
            new MinerItemBonus
            {
                MinLevel = 40,
                Value = 4
            },
            new MinerItemBonus
            {
                MinLevel = 50,
                Value = 5
            },
            new MinerItemBonus
            {
                MinLevel = 60,
                Value = 6
            },
            new MinerItemBonus
            {
                MinLevel = 70,
                Value = 7
            },
            new MinerItemBonus
            {
                MinLevel = 80,
                Value = 8
            },
            new MinerItemBonus
            {
                MinLevel = 90,
                Value = 9
            },
            new MinerItemBonus
            {
                MinLevel = 100,
                Value = 10
            },
        };
        public static List<MinerBlacksmithItemBonus> MinerBlacksmithItemBonuses => new List<MinerBlacksmithItemBonus>
        {
            new MinerBlacksmithItemBonus
            {
                MinLevel = 20,
                Value = 20
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 50,
                Value = 60
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 70,
                Value = 85
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 100,
                Value = 110
            },
        };
        public static List<MinerProspectorItemBonus> MinerProspectorItemBonuses => new List<MinerProspectorItemBonus>
        {
            new MinerProspectorItemBonus
            {
                MinLevel = 20,
                Chance = 0.5,
                Value = 2
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 30,
                Chance = 0.75,
                Value = 3
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 50,
                Chance = 0.75,
                Value = 5
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 70,
                Chance = 1.0,
                Value = 10
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 100,
                Chance = 1.0,
                Value = 15
            },
        };
        public static List<GeologistItemBonus> GeologistItemBonuses => new List<GeologistItemBonus>
        {
            new GeologistItemBonus
            {
                MinLevel = 20,
                Chance = 0.30,
                Value = 1
            },
            new GeologistItemBonus
            {
                MinLevel = 30,
                Chance = 0.40,
                Value = 1
            },
            new GeologistItemBonus
            {
                MinLevel = 50,
                Chance = 0.50,
                Value = 2
            },
            new GeologistItemBonus
            {
                MinLevel = 70,
                Chance = 0.50,
                Value = 2
            },
            new GeologistItemBonus
            {
                MinLevel = 100,
                Chance = 0.50,
                Value = 3
            },
        };
        public static List<GeologistExcavatorItemBonus> GeologistExcavatorItemBonuses => new List<GeologistExcavatorItemBonus>
        {
            new GeologistExcavatorItemBonus
            {
                MinLevel = 20,
                Chance = 0.50,
                Value = 2
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 30,
                Chance = 0.75,
                Value = 2
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 50,
                Chance = 0.75,
                Value = 3
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 70,
                Chance = 0.8,
                Value = 3
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 100,
                Chance = 0.8,
                Value = 4
            },
        };
        public static List<GeologistGemologistItemBonus> GeologistGemologistItemBonuses => new List<GeologistGemologistItemBonus>
        {
            new GeologistGemologistItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 50,
                Value = 50
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 100,
                Value = 70
            },
        };
        public static bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item)
        {
            /// TODO: I'm sure this can be optimized :) ... skip Professions with no "Item Drop Rate" etc.
            var ret = false;
            ret = ItemBonuses.ApplyMoreDrops(MinerItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(MinerBlacksmithItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(MinerProspectorItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(GeologistItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(GeologistExcavatorItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(GeologistGemologistItemBonuses, skills, item) || ret;
            return ret;
        }
        public static bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            /// TODO: I'm sure this can be optimized :) ... skip Professions with no "Item Worth More" etc.
            var ret = false;
            ret = ItemBonuses.ApplyWorthMore(MinerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(MinerBlacksmithItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(MinerProspectorItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(GeologistItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(GeologistExcavatorItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(GeologistGemologistItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            return ret;
        }
    }
}
