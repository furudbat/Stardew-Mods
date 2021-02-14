using System.Collections.Generic;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    public partial class Profession
    {
        /*********
        ** Accessors
        *********/
        public static IEnumerable<Profession> FarmingProfessions => new List<Profession>
        {
            Rancher,
            Tiller,
            Coopmaster,
            Shepherd,
            Artisan,
            Agriculturist
        };
        protected static TierOneProfession Rancher { get; set; }
        protected static TierOneProfession Tiller { get; set; }
        protected static TierTwoProfession Coopmaster { get; set; }
        protected static TierTwoProfession Shepherd { get; set; }
        protected static TierTwoProfession Artisan { get; set; }
        protected static TierTwoProfession Agriculturist { get; set; }
    }
}
