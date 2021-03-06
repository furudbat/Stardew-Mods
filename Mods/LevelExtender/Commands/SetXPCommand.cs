﻿using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class SetXPCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public SetXPCommand()
            : base("set_xp", GetDescription()) { }

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

            string skillName = args[0].ToLower();
            int value;
            bool validLevel = int.TryParse(args[1], out value);
            if (!validLevel && value >= 0)
            {
                Logger.LogInformation("<number> must be a valid number, >= 0");
                return;
            }

            var succ = Mod.SetVanillaXP(skillName, value);
            if (succ)
            {
                Logger.LogInformation($"SetXPCommand: Set skill XP for {skillName} to {value}");
            }
            else
            {
                Logger.LogInformation($"SetXPCommand: Can't find skill {skillName} (must be vanilla skill)");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Sets your current XP for a given skill: set_xp <skill name> <XP: int 0 -> ANY>.";
        }
    }
}
