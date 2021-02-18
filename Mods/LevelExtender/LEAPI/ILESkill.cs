namespace LevelExtender.LEAPI
{
    public static class DefaultSkillNames
    {
        public const string Farming = "Farming";
        public const string Fishing = "Fishing";
        public const string Foraging = "Foraging";
        public const string Mining = "Mining";
        public const string Combat = "Combat";
    }

    public interface ILESkill
    {
        string Name { get; }
        int Level { get; }
        int XP { get; }
        int RequiredXPNextLevel { get; }
    }
}
