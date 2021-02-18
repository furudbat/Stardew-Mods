using System;
using System.Collections.Generic;
using StardewValley;


namespace LevelExtender.LEAPI
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

        public Func<IEnumerable<int>> ExtraItemCategories { get; set; }

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
    }
}
