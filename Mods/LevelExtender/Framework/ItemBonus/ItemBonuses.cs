using System.Collections.Generic;
using System.Linq;
using LevelExtender.Common;

namespace LevelExtender.Framework.ItemBonus
{
    static class ItemBonuses
    {
        public static bool ApplyMoreDrops<T>(List<T> itemBonuses, IEnumerable<LESkill> skills, StardewValley.Object item) where T : ItemBonusFromSkill
        {
            itemBonuses.Sort((a, b) => b.MinLevel.CompareTo(a.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Type == itemBonus.skillType);
                var skillLevel = (skill != null) ? skill.Level : -1;
                if (skillLevel < itemBonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyMoreDrops(skills, item))
                {
                    ModEntry.Logger.LogDebug($"ItemBonuses.ApplyMoreDrops: skill: {skill?.Name} {itemBonus.MinLevel} {itemBonus.ItemBonusType}; item: {item.name} Q:{item.Quality} Stk:{item.Stack} ");
                    return true;
                }
            }

            return false;
        }
        public static bool ApplyWorthMore<T>(List<T> itemBonuses, IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice) where T : ItemBonusFromSkill
        {
            itemBonuses.Sort((a, b) => b.MinLevel.CompareTo(a.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Type == itemBonus.skillType);
                var skillLevel = (skill != null) ? skill.Level : -1;
                if (skillLevel < itemBonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyWorthMore(skills, item, specificPlayerID, ref newprice))
                {
                    //ModEntry.Logger.LogDebug($"ItemBonuses.ApplyWorthMore: skill: {skill?.Name} {itemBonus.MinLevel} {itemBonus.ItemBonusType}; item: {item.name} new price: {newprice} ");
                    return true;
                }
            }

            return false;
        }
    }
}

