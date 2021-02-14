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
                ModEntry.LogMonitor.Log("<skill name> must be specified");
                return;
            }

            if (args.Length < 2)
            {
                ModEntry.LogMonitor.Log("<number> must be specified");
                return;
            }

            string skill_name = args[0];
            double value = 1.0;
            bool validValue = double.TryParse(args[1], out value);
            if (!validValue)
            {
                ModEntry.LogMonitor.Log("<value> must be a decimal");
                return;
            }

            var succ = Mod.SetNeededXPFactor(skill_name, value);
            if (succ)
            {
                ModEntry.LogMonitor.Log($"SetNeededXPFactorCommand: Set skill needed xp factor for {args[0]} to {args[1]}");
            }

            ModEntry.LogMonitor.Log($"SetNeededXPFactorCommand: Can't find skill {args[0]}");
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Changes the needed xp factor for a given skill: xp_m <skill name> <decimal value 0.0 -> ANY>: 1.0 is default.";
        }
    }
}
