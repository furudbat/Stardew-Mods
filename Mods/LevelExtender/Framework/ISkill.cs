using StardewValley;

namespace LevelExtender.Framework
{
    interface ISkill
    {
        string Id { get; }
        string GetName();
        int[] ExperienceCurve { get; set; }
        int GetExperienceFor(Farmer farmer);
        int GetSkillLevel(Farmer farmer);
    }
}
