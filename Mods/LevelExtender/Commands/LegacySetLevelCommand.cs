using LevelExtender.Common;
using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class LegacySetLevelCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public LegacySetLevelCommand()
            : base("lev", GetDescription()) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogInformation("<skill name> must be specified");
                return;
            }

            if (args.Length < 2)
            {
                Logger.LogInformation("<number> must be specified");
                return;
            }

            string skill_name = args[0];
            bool validLevel = int.TryParse(args[1], out int value);
            if (!validLevel)
            {
                Logger.LogInformation("<number> must be a number");
                return;
            }

            var succ = Mod.SetLevel(skill_name, value);
            if (succ)
            {
                Logger.LogInformation($"SetLevelCommand: Set skill Level for {skill_name} to {value}");
            }
            else
            {
                Logger.LogInformation($"SetLevelCommand: Can't find skill {skill_name}");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Sets the player's level: lev <skill name> <number>.";
        }
    }
}
