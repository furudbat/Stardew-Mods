using System.Collections.Generic;
using System.Linq;
using LevelExtender.Framework.SkillTypes;
using LevelExtender.LEAPI;
using StardewValley;

namespace LevelExtender.Framework
{
    public static class SkillsList
    {

        /// <summary>The default skills available in the unmodded game.</summary>
        public static IEnumerable<Skill> DefaultSkills => new List<Skill>
        {
            new Skill
            {
                Type = DefaultSkillTypes.Farming,
                SetSkillLevel = level => Game1.player.FarmingLevel = level,
                GetSkillLevel = () => Game1.player.FarmingLevel,
                ExtraItemCategories = () => new List<int> { Object.SeedsCategory, Object.VegetableCategory, Object.FruitsCategory, Object.flowersCategory }
            },
            new Skill
            {
                Type = DefaultSkillTypes.Fishing,
                SetSkillLevel = level => Game1.player.FishingLevel = level,
                GetSkillLevel = () => Game1.player.FishingLevel,
                ExtraItemCategories = () => new List<int> { Object.FishCategory }
            },
            new Skill
            {
                Type = DefaultSkillTypes.Foraging,
                SetSkillLevel = level => Game1.player.ForagingLevel = level,
                GetSkillLevel = () => Game1.player.ForagingLevel,
                ExtraItemCategories = () => new List<int> { Object.buildingResources, Object.GreensCategory }
            },
            new Skill
            {
                Type = DefaultSkillTypes.Mining,
                SetSkillLevel = level => Game1.player.MiningLevel = level,
                GetSkillLevel = () => Game1.player.MiningLevel,
                ExtraItemCategories = () => new List<int> { Object.GemCategory, Object.mineralsCategory, Object.metalResources }
            },
            new Skill
            {
                Type = DefaultSkillTypes.Combat,
                SetSkillLevel = level => Game1.player.CombatLevel = level,
                GetSkillLevel = () => Game1.player.CombatLevel,
                ExtraItemCategories = () => new List<int> { Object.monsterLootCategory, Object.equipmentCategory, Object.hatCategory, Object.ringCategory, Object.weaponCategory }
            }
        };

        /// <summary>Returns all skills loaded and registered into this mod, default and mod.</summary>
        public static IEnumerable<Skill> AllSkills
        {
            get
            {
                var skills = new List<Skill>(DefaultSkills);
                var addedSkills = LEModHandler.GetAddedSkills().ToList();
                if (addedSkills.Any())
                    skills.AddRange(addedSkills);
                return skills;
            }
        }
    }
}
