using System.Collections.Generic;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    public partial class Profession
    {
        /*********
        ** Accessors
        *********/
        public static IEnumerable<Profession> FishingProfessions => new List<Profession>
        {
            Fisher,
            Trapper,
            Angler,
            Pirate,
            Mariner,
            Luremaster
        };

        protected static TierOneProfession Fisher { get; set; }
        protected static TierOneProfession Trapper { get; set; }
        protected static TierTwoProfession Angler { get; set; }
        protected static TierTwoProfession Pirate { get; set; }
        protected static TierTwoProfession Mariner { get; set; }
        protected static TierTwoProfession Luremaster { get; set; }
    }
}
