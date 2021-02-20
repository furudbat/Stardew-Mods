using System;
using System.Linq;
using LevelExtender.Common;
using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Commands
{
    internal class ShowXPTableSkillCommand : SMAPICommand<ILevelExtender>
    {
        /// <summary>Construct an instance.</summary>
        public ShowXPTableSkillCommand()
            : base("show_xp_table", GetDescription(), true) { }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected override void Apply(string[] args)
        {
            if (args.Length < 1)
            {
                Logger.LogInformation("<skill name> must be specified");
                return;
            }

            string skill_name = args[0];
            var skill = Mod.Skills.FirstOrDefault(s => s.Name.ToLower() == skill_name.ToLower());
            if (skill != null)
            {
                Logger.LogInformation($"Skill ${skill.Name}, LvL: {skill.Level}, XP: {skill.XP}");

                var table = new System.Text.StringBuilder();
                table.Append(String.Format("{0,4} {1,15}\n\n", "Level", "Req. XP"));
                for (int level = skill.Level; level < skill.XPTable.Count;level++) {
                    var reqxp = skill.XPTable[level];
                    table.Append(String.Format("{0,4} {1,15}\n", level, reqxp));
                }
                Logger.LogInformation(table.ToString());
            } else {
                Logger.LogInformation($"ShowXPTableSkillCommand: Skill {skill_name} not found!");
            }
        }

        /// <summary>Get the command's help description.</summary>
        private static string GetDescription()
        {
            return "Displays the XP table for a given skill: show_xp_table <skill name>";
        }
    }
}
