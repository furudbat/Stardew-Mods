using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelExtender.Common;
using StardewModdingAPI;

namespace LevelExtender.Framework.ItemBonus
{
    internal class GathererItemBonus : ItemBonusFromSkill
    {
        public GathererItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Foraging;
            ItemBonusType = ItemBonusType.GathererMoreForage;
            ItemCategories = DefaultItemCategories.Foraging;
        }
    }
    internal class ForesterTapperItemBonus : ItemBonusFromSkill
    {
        public ForesterTapperItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Foraging;
            ItemBonusType = ItemBonusType.ForesterTapperSyrupWorthMore;
            ItemCategories = DefaultItemCategories.Syrup;
        }
    }

    internal static class ForagingItemBonuses
    {
        public static List<GathererItemBonus> GathererItemBonuses => new List<GathererItemBonus>
        {
            new GathererItemBonus
            {
                MinLevel = 10,
                Chance = 0.25,
                Value = 25
            },
            new GathererItemBonus
            {
                MinLevel = 20,
                Chance = 0.25,
                Value = 2
            },
            new GathererItemBonus
            {
                MinLevel = 30,
                Chance = 0.30,
                Value = 2
            },
            new GathererItemBonus
            {
                MinLevel = 50,
                Chance = 0.40,
                Value = 3
            },
            new GathererItemBonus
            {
                MinLevel = 70,
                Chance = 0.40,
                Value = 3
            },
            new GathererItemBonus
            {
                MinLevel = 100,
                Chance = 0.60,
                Value = 4
            },
        };
        public static List<ForesterTapperItemBonus> ForesterTapperItemBonuses => new List<ForesterTapperItemBonus>
        {
            new ForesterTapperItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 30,
                Value = 35
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 50,
                Value = 40
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 70,
                Value = 45
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 100,
                Value = 50
            },
        };
        public static bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item) 
        {
            var ret = false;
            ret = ItemBonuses.ApplyMoreDrops(GathererItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(ForesterTapperItemBonuses, skills, item) || ret;
            return ret;
        }
        public static bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var ret = false;
            ret = ItemBonuses.ApplyWorthMore(GathererItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(ForesterTapperItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            return ret;
        }
    }
}
