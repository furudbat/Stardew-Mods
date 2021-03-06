using LevelExtender.Framework;
using LevelExtender.Logging;
using StardewValley;

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
                Logger.LogInformation($"{skill.Id} | {skill.GetSkillLevel(Game1.player)} | {skill.GetSkillLevel(Game1.player)} | {skill.GetRequiredXPNextLevel(Game1.player)}");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Displays the xp table for your current skill levels.";
        }
    }
}
