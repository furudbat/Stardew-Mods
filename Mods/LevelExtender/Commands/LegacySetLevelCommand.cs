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
                ModEntry.LogMonitor.Log("<skill name> must be specified");
                return;
            }

            if (args.Length < 2)
            {
                ModEntry.LogMonitor.Log("<number> must be specified");
                return;
            }

            string skill_name = args[0];
            int value = -1;
            bool validLevel = int.TryParse(args[1], out value);
            if (!validLevel)
            {
                ModEntry.LogMonitor.Log("<number> must be a nuber");
                return;
            }

            var succ = Mod.SetLevel(skill_name, value);
            if (succ)
            {
                ModEntry.LogMonitor.Log($"SetLevelCommand: Set skill Level for {skill_name} to {value}");
            }

            ModEntry.LogMonitor.Log($"SetLevelCommand: Can't find skill {args[0]}");
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Sets the player's level: lev <skill name> <number>.";
        }
    }
}
