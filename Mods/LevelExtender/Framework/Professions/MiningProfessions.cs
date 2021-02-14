using System.Collections.Generic;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    public partial class Profession
    {
        /*********
        ** Accessors
        *********/
        public static IEnumerable<Profession> MiningProfessions => new List<Profession>
        {
            Miner,
            Geologist,
            Blacksmith,
            Prospector,
            Excavator,
            Gemologist
        };

        protected static TierOneProfession Miner { get; set; }
        protected static TierOneProfession Geologist { get; set; }
        protected static TierTwoProfession Blacksmith { get; set; }
        protected static TierTwoProfession Prospector { get; set; }
        protected static TierTwoProfession Excavator { get; set; }
        protected static TierTwoProfession Gemologist { get; set; }
    }
}
