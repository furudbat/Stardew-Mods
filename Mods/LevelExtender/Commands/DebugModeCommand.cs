using System;
using System.Linq;
using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class DebugModeCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public DebugModeCommand()
            : base("le_debug", GetDescription(), true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            bool debug = false;
            bool validMode = (args.Length > 0) && bool.TryParse(args[0], out debug);
            if (validMode && debug)
            {
                this.Mod.EditConfig((config) => config.LogLevel = LogLevel.Debug);
                Logger.LogInformation("Debug ON");
            }
            else
            {
                this.Mod.EditConfig((config) => config.LogLevel = LogLevel.Information);
                Logger.LogInformation("Debug OFF");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Enable Debug Mode ON/OFF (change LogLevel for this Mod)";
        }
    }
}
