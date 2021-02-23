using LevelExtender.Common;
using LevelExtender.Framework;

namespace LevelExtender.Commands
{
    internal class LegacySetNeededXPFactorCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public LegacySetNeededXPFactorCommand()
            : base("xp_m", GetDescription()) { }

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
            bool validValue = double.TryParse(args[1], out double value);
            if (!validValue)
            {
                Logger.LogInformation("<value> must be a decimal");
                return;
            }

            var succ = Mod.SetNeededXPFactor(skill_name, value);
            if (succ)
            {
                Logger.LogInformation($"SetNeededXPFactorCommand: Set skill needed xp factor for {args[0]} to {args[1]}");
            }
            else
            {
                Logger.LogInformation($"SetNeededXPFactorCommand: Can't find skill {skill_name}");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Changes the needed xp factor for a given skill: xp_m <skill name> <decimal value 0.0 -> ANY>: 1.0 is default.";
        }
    }
}
