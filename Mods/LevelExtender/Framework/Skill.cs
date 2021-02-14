using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.Framework.Mods;
using LevelExtender.Framework.Professions;
using LevelExtender.Framework.SkillTypes;
using StardewValley;


namespace LevelExtender.Framework
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Represents a skill in Stardew Valley.</summary>
    public class Skill
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Metadata about a skill type.</summary>
        public SkillType Type { get; set; }

        /// <summary>The professions for this skill.</summary>
        public IEnumerable<Profession> Professions { get; set; }

        /// <summary>An action to set the skill's level. For the unmodded game, this sets the relevant player field (e.g. <see cref="Farmer.farmingLevel"/>). If you are implementing this class for your mod it should be whatever would be needed to set the skill level to a given integer.</summary>
        public Action<int> SetSkillLevel;

        /// <summary>A function to return the skill's level. For the unmodded game. this gets the relevant player field (e.g. <see cref="Farmer.farmingLevel"/>). If you are implementing this class for your mod it should be whatever would be needed to retrieve the player's current skill level.</summary>
        public Func<int> GetSkillLevel;

        /// <summary>An action to get the skill's experience. For the unmodded game, this updates the <see cref="Farmer.experiencePoints"/> array based on <see cref="SkillType.Ordinal"/>. If you are implementing this class for your mod it should be whatever would be needed to set the skill experience level to a given integer.</summary>
        // ReSharper disable once MemberCanBePrivate.Global used by other mods.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Action<int> SetSkillExperience { get; set; }

        /// <summary>An action to get the skill's experience. For the unmodded game, this reads the <see cref="Farmer.experiencePoints"/> array based on <see cref="SkillType.Ordinal"/>.</summary>
        public Func<int> GetSkillExperience { get; set; }

        /// <summary>The default skills available in the unmodded game.</summary>
        public static IEnumerable<Skill> DefaultSkills => new List<Skill>
        {
            new Skill
            {
                Type = SkillType.Farming,
                Professions = Profession.FarmingProfessions,
                SetSkillLevel = level => Game1.player.FarmingLevel = level,
                GetSkillLevel = () => Game1.player.FarmingLevel
            },
            new Skill
            {
                Type = SkillType.Fishing,
                Professions = Profession.FishingProfessions,
                SetSkillLevel = level => Game1.player.FishingLevel = level,
                GetSkillLevel = () => Game1.player.FishingLevel
            },
            new Skill
            {
                Type = SkillType.Foraging,
                Professions = Profession.ForagingProfessions,
                SetSkillLevel = level => Game1.player.ForagingLevel = level,
                GetSkillLevel = () => Game1.player.ForagingLevel
            },
            new Skill
            {
                Type = SkillType.Mining,
                Professions = Profession.MiningProfessions,
                SetSkillLevel = level => Game1.player.MiningLevel = level,
                GetSkillLevel = () => Game1.player.MiningLevel
            },
            new Skill
            {
                Type = SkillType.Combat,
                Professions = Profession.CombatProfessions,
                SetSkillLevel = level => Game1.player.CombatLevel = level,
                GetSkillLevel = () => Game1.player.CombatLevel
            }
        };

        /// <summary>Returns all skills loaded and registered into this mod, default and mod.</summary>
        public static IEnumerable<Skill> AllSkills
        {
            get
            {
                var skills = new List<Skill>(DefaultSkills);
                var addedSkills = ModHandler.GetAddedSkills().ToList();
                if (addedSkills.Any())
                    skills.AddRange(addedSkills);
                return skills;
            }
        }

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public Skill()
        {
            GetSkillExperience = () => Game1.player.experiencePoints[Type.Ordinal];
            SetSkillExperience = exp =>
            {
                Game1.player.experiencePoints[Type.Ordinal] = 0;
                Game1.player.gainExperience(Type.Ordinal, exp);
            };
        }

        /// <summary>Get the unique IDs for the skill's professions.</summary>
        public IEnumerable<int> GetAllProfessionIds()
        {
            return Professions.Select(x => x.Id);
        }
    }
}
