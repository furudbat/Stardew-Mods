using LevelExtender.Common;
using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class LegacyShowXPTableCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public LegacyShowXPTableCommand()
            : base("xp", GetDescription(), true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            Logger.LogDisplay("Skill:  | Level:  |  Current Experience:  | Experience Needed:");

            foreach (var skill in Mod.Skills)
            {
                Logger.LogInformation($"{skill.Name} | {skill.Level} | {skill.XP} | {skill.RequiredXPNextLevel}");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Displays the xp table for your current skill levels.";
        }
    }
}
