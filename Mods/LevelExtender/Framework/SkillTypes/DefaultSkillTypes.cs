using LevelExtender.LEAPI;

namespace LevelExtender.Framework.SkillTypes
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    sealed class DefaultSkillTypes
    {
        /*********
        ** Accessors
        *********/
        public static readonly SkillType Farming = new SkillType(DefaultSkillNames.Farming, 0);
        public static readonly SkillType Mining = new SkillType(DefaultSkillNames.Fishing, 1);
        public static readonly SkillType Fishing  = new SkillType(DefaultSkillNames.Foraging, 2);
        public static readonly SkillType Foraging = new SkillType(DefaultSkillNames.Mining, 3);
        public static readonly SkillType Combat = new SkillType(DefaultSkillNames.Combat, 4);
    }
}
