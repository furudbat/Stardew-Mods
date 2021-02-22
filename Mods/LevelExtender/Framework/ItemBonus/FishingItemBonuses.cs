using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelExtender.Common;
using StardewModdingAPI;

namespace LevelExtender.Framework.ItemBonus
{
    internal class FisherItemBonus : ItemBonusFromSkill
    {
        public FisherItemBonus()
        {
            SkillType = SkillTypes.DefaultSkillTypes.Fishing;
            ItemBonusType = ItemBonusType.FisherFishWorthMore;
            ItemCategories = DefaultItemCategories.Fish;
        }
    }

    internal static class FishingItemBonuses
    {
        public static List<FisherItemBonus> FisherAnglerItemBonuses => new List<FisherItemBonus>
        {
            new FisherItemBonus
            {
                MinLevel = 20,
                Value = 75
            },
            new FisherItemBonus
            {
                MinLevel = 30,
                Value = 90
            },
            new FisherItemBonus
            {
                MinLevel = 50,
                Value = 100
            },
            new FisherItemBonus
            {
                MinLevel = 70,
                Value = 120
            },
            new FisherItemBonus
            {
                MinLevel = 100,
                Value = 150
            },
        };
        public static bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item) 
        {
            var ret = false;
            ret = ItemBonuses.ApplyMoreDrops(FisherAnglerItemBonuses, skills, item) || ret;
            return ret;
        }
        public static bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var ret = false;
            ret = ItemBonuses.ApplyWorthMore(FisherAnglerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            return ret;
        }
    }
}
