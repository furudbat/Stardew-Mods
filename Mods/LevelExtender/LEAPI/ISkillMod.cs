using System.Collections.Generic;

namespace LevelExtender.LEAPI
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>
    /// Interface that all skill mods need to implement in order to register with Skill
    /// </summary>
    public interface ISkillMod
    {
        /// <summary>
        /// The name to display for the mod in the log.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Skills to add to the system.
        /// </summary>
        IEnumerable<Skill> AdditionalSkills { get; }
        bool IsFound { get; }
    }
}
