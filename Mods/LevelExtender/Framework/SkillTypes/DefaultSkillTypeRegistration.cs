

namespace LevelExtender.Framework.SkillTypes
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>
    /// Class registers all of the skill types that are available in Stardew Valley by default.
    /// </summary>
    // ReSharper disable once UnusedMember.Global - created through reflection.
    public sealed class DefaultSkillTypeRegistration : SkillType, ISkillTypeRegistration
    {
        public void RegisterSkillTypes()
        {
            Farming = new SkillType("Farming", 0);
            Fishing = new SkillType("Fishing", 1);
            Foraging = new SkillType("Foraging", 2);
            Mining = new SkillType("Mining", 3);
            Combat = new SkillType("Combat", 4);
        }

    }
}

