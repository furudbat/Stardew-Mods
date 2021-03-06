using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class LegacyToggleOverworldMonstersCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public LegacyToggleOverworldMonstersCommand()
            : base("wm_toggle", GetDescription()) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            var config = Mod.EditConfig(conf => conf.OverworldMonsters = !conf.OverworldMonsters);
            if (config.OverworldMonsters)
            {
                Logger.LogInformation($"Overworld Monster Spawning -> ON.");
            }
            else
            {
                Logger.LogInformation($"Overworld Monster Spawning -> OFF.");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Toggles monster spawning: wm_toggle.";
        }
    }
}
