using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Framework.ItemBonus
{
    static class ItemQuality
    {
        public static readonly int Regular = StardewValley.Object.lowQuality;
        public static readonly int Silver = StardewValley.Object.medQuality;
        public static readonly int Gold = StardewValley.Object.highQuality;
        public static readonly int Irudium = StardewValley.Object.bestQuality;
    }
    static class ItemBonuses
    {
        public static bool ApplyMoreDrops<T>(Farmer farmer, List<T> itemBonuses, IEnumerable<BaseSkill> skills, StardewValley.Object item) where T : ItemBonusBySkill
        {
            itemBonuses.Sort((a, b) => b.Bonus.MinLevel.CompareTo(a.Bonus.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Id == itemBonus.Bonus.SkillId);
                var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;
                if (skillLevel < itemBonus.Bonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyMoreDrops(farmer, skills, item))
                {
                    ModEntry.Logger.LogDebug($"ItemBonuses.ApplyMoreDrops: skill: {skill?.Id} minLvL. {itemBonus.Bonus.MinLevel} prof. {itemBonus.Bonus.Profession}; item: {item.name} Q:{item.Quality} Stk:{item.Stack} ");
                    return true;
                }
            }

            return false;
        }
        public static bool ApplyWorthMore<T>(Farmer farmer, List<T> itemBonuses, IEnumerable<BaseSkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice) where T : ItemBonusBySkill
        {
            itemBonuses.Sort((a, b) => b.Bonus.MinLevel.CompareTo(a.Bonus.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Id == itemBonus.Bonus.SkillId);
                var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;
                if (skillLevel < itemBonus.Bonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyWorthMore(farmer, skills, item, specificPlayerID, ref newprice))
                {
                    //ModEntry.Logger.LogDebug($"ItemBonuses.ApplyWorthMore: skill: {skill?.Name} {itemBonus.MinLevel} {itemBonus.ItemBonusType}; item: {item.name} new price: {newprice} ");
                    return true;
                }
            }

            return false;
        }
        public static bool ApplyBetterQuality<T>(Farmer farmer, List<T> itemBonuses, IEnumerable<BaseSkill> skills, StardewValley.Object item) where T : ItemBonusBySkill
        {
            itemBonuses.Sort((a, b) => b.Bonus.MinLevel.CompareTo(a.Bonus.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Id == itemBonus.Bonus.SkillId);
                var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;
                if (skillLevel < itemBonus.Bonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyBetterQuality(farmer, skills, item))
                {
                    ModEntry.Logger.LogDebug($"ItemBonuses.ApplyBetterQuality: skill: {skill?.Id} minLvL. {itemBonus.Bonus.MinLevel} prof. {itemBonus.Bonus.Profession}; item: {item.name} new Q: {item.Quality} ");
                    return true;
                }
            }

            return false;
        }

        public static bool ApplyCropGrow<T>(Farmer farmer, List<T> itemBonuses, List<BaseSkill> skills, HoeDirt hoeDirt) where T : ItemBonusBySkill
        {
            itemBonuses.Sort((a, b) => b.Bonus.MinLevel.CompareTo(a.Bonus.MinLevel));
            foreach (var itemBonus in itemBonuses)
            {
                var skill = skills.FirstOrDefault(s => s.Id == itemBonus.Bonus.SkillId);
                var skillLevel = (skill != null) ? skill.GetSkillLevel(farmer) : -1;
                if (skillLevel < itemBonus.Bonus.MinLevel)
                {
                    continue;
                }

                if (itemBonus.ApplyCropGrow(farmer, skills, hoeDirt))
                {
                    ModEntry.Logger.LogDebug($"ItemBonuses.ApplyCropGrow: crop phase: {hoeDirt?.crop?.currentPhase} minLvL. {itemBonus.Bonus.MinLevel} prof. {itemBonus.Bonus.Profession}; day: {hoeDirt?.crop?.dayOfCurrentPhase} phases: {hoeDirt?.crop?.phaseDays?.Count} ");
                    return true;
                }
            }

            return false;
        }

        public static void RegisterItemBonus<T>(IEnumerable<T> itemBonuses) where T : ItemBonusBySkillData
        {
            RegisteredItemBonuses.Add(itemBonuses.Select(it => new ItemBonusBySkill(it)));
        }

        static ItemBonuses()
        {
            ModEntry.Logger.LogInformation("Registering item bonuses...");
            var concreteItemBonusFromSkillRegistrations = AppDomain.CurrentDomain.GetNonSystemAssemblies().SelectMany(x => x.GetTypesSafely()).Where(x => typeof(IItemBonusesRegistration).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToList();
            ModEntry.Logger.LogVerbose($"concerete item bonuses registration count: {concreteItemBonusFromSkillRegistrations.Count}");
            foreach (var registration in concreteItemBonusFromSkillRegistrations)
            {
                ModEntry.Logger.LogVerbose($"Creating instance of type {registration.FullName}...");
                foreach (var itemBonuses in ((IItemBonusesRegistration)Activator.CreateInstance(registration)).RegisterItemBonuses())
                {
                    RegisterItemBonus(itemBonuses);
                    ModEntry.Logger.LogVerbose($"RegisteredItemBonuses added {itemBonuses.GetType().FullName}...");
                }
            }
            ModEntry.Logger.LogInformation("Item bonuses registered.");
        }

        public static List<IEnumerable<ItemBonusBySkill>> RegisteredItemBonuses { get; private set; } = new List<IEnumerable<ItemBonusBySkill>>();
    }
}

