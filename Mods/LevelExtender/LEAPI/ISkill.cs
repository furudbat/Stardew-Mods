using System;
using System.Collections.Generic;

namespace LevelExtender.LEAPI
{
    public interface ISkill
    {
        SkillType Type { get; set; }
        Action<int> SetSkillLevel { get; set; }
        Func<int> GetSkillLevel { get; set; }
        Action<int> SetSkillExperience { get; set; }
        Func<int> GetSkillExperience { get; set; }
        Func<IEnumerable<int>> ExtraItemCategories { get; set; }
    }
}
