using System.Collections.Generic;
using StardewValley;

namespace LevelExtender.Framework
{
    public static class SkillsList
    {
        public static class DefaultSkillNames
        {
            public const string Farming = "farming";
            public const string Fishing = "fishing";
            public const string Foraging = "foraging";
            public const string Mining = "mining";
            public const string Combat = "combat";
        }
        /// <summary>The default skills available in the unmodded game.</summary>
        internal static IEnumerable<VanillaSkill> DefaultSkills => new List<VanillaSkill>
        {
            new VanillaSkill(DefaultSkillNames.Farming, Farmer.farmingSkill)
            {
                SetSkillLevel = level => Game1.player.FarmingLevel = level,
                GetSkillLevel = () => Game1.player.FarmingLevel,
                ExtraItemCategories = () => new List<int> { Object.SeedsCategory, Object.VegetableCategory, Object.FruitsCategory, Object.flowersCategory }
            },
            new VanillaSkill(DefaultSkillNames.Fishing, Farmer.fishingSkill)
            {
                SetSkillLevel = level => Game1.player.FishingLevel = level,
                GetSkillLevel = () => Game1.player.FishingLevel,
                ExtraItemCategories = () => new List<int> { Object.FishCategory }
            },
            new VanillaSkill(DefaultSkillNames.Foraging, Farmer.foragingSkill)
            {
                SetSkillLevel = level => Game1.player.ForagingLevel = level,
                GetSkillLevel = () => Game1.player.ForagingLevel,
                ExtraItemCategories = () => new List<int> { Object.buildingResources, Object.GreensCategory }
            },
            new VanillaSkill(DefaultSkillNames.Mining, Farmer.miningSkill)
            {
                SetSkillLevel = level => Game1.player.MiningLevel = level,
                GetSkillLevel = () => Game1.player.MiningLevel,
                ExtraItemCategories = () => new List<int> { Object.GemCategory, Object.mineralsCategory, Object.metalResources }
            },
            new VanillaSkill(DefaultSkillNames.Combat, Farmer.combatSkill)
            {
                SetSkillLevel = level => Game1.player.CombatLevel = level,
                GetSkillLevel = () => Game1.player.CombatLevel,
                ExtraItemCategories = () => new List<int> { Object.monsterLootCategory, Object.equipmentCategory, Object.hatCategory, Object.ringCategory, Object.weaponCategory }
            }
        };
    }
}
