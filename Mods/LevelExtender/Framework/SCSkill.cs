using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace LevelExtender.Framework
{
    class SCSkill : BaseSkill
    {
        public readonly SpaceCore.Skills.Skill Skill;

        public SCSkill(SpaceCore.Skills.Skill skill, double? needed_xp_factor = null) {
            Skill = skill;
            XPTable = skill.ExperienceCurve.ToList();
            if (needed_xp_factor != null)
            {
                NeededXPFactor = needed_xp_factor.Value;
            }
            GenerateXPTable(MaxLevel + 1);

            ModEntry.Logger.LogDebug($"SCSkill: {Id}; xpt count: {XPTable.Count}");

            foreach (int lvl in new List<int> { 0, 1, 2, 3, 5, 9, 10, 11, 14, 15, 16, 20, 25, 26, 30, 40, 44, 45, 46, 50, 60, 70, 80, 90, 99, 100, 101 })
            {
                ModEntry.Logger.LogDebug(String.Format("SCSkill: XP Table: {0, 3} -> {1, 3} => {2, 10}", lvl, lvl + 1, GetRequiredXP(lvl)));
            }
        }

        public override string Id => Skill.Id;

        public override int GetExperienceFor(Farmer farmer)
        {
            return SpaceCore.Skills.GetExperienceFor(farmer, Skill.GetName());
        }

        public override string GetName()
        {
            return Skill.GetName();
        }

        public override int GetSkillLevel(Farmer farmer)
        {
            return SpaceCore.Skills.GetSkillLevel(farmer, Skill.GetName());
        }
    }
}
