using System.Collections.Generic;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Represents a Tier 1 profession in Stardew Valley (a profession available at level 5).</summary>
    public class TierOneProfession : Profession
    {
        /*********
        ** Accessors
        *********/
        public override int LevelAvailableAt => 5;

        /// <summary>The tier two (available at level 10) professions that are available when this profession is chosen.</summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IEnumerable<TierTwoProfession> TierTwoProfessions { get; set; }
    }
}
