using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class SetLevelCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public SetLevelCommand()
            : base("set_level", GetDescription()) { }

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
                Logger.LogInformation("<level> must be specified");
                return;
            }

            string skill_name = args[0];
            int value = -1;
            bool validLevel = int.TryParse(args[1], out value);
            if (!validLevel && value >= 0)
            {
                Logger.LogInformation("<level> must be a valid number, >= 0");
                return;
            }

            var succ = Mod.SetLevel(skill_name, value);
            if (succ)
            {
                Logger.LogInformation($"SetLevelCommand: Set skill Level for {skill_name} to {value}");
            }
            else
            {
                Logger.LogInformation($"SetLevelCommand: Can't find skill {args[0]}");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Sets the player's level: set_level <skill name> <level>.";
        }
    }
}
