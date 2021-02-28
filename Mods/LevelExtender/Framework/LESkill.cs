using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.LEAPI;

namespace LevelExtender.Framework
{
    public class LESkill : ILESkill
    {
        public readonly int MaxLevel = 100;

        public readonly ISkill Skill;
        public LEEvents LEEvents { get; private set; }
        public bool SetXPByLevel { get; private set; } = false;
        public int ChangedXP { get; private set; } = 0;
        public List<int> XPTable { get; private set; } = new List<int>();
        public double NeededXPFactor { get; set; } = 1.0;

        public SkillType Type
        {
            get
            {
                return Skill.Type;
            }
        }
        public string Name
        {
            get
            {
                return Skill.Type.Name;
            }
        }

        public List<int> ItemCategories { get; private set; } = new List<int>();
        public int MaxXP { get; private set; } = 0;

        public int XP
        {
            get { return _xp; }
            set
            {
                if (_xp != value)
                {
                    value = Math.Max(0, Math.Min(MaxXP, value));
                    ModEntry.Logger.LogDebug($"LESkill: {Name}, set XP {value}");

                    int oldxp = _xp;
                    ChangedXP = value - _xp;
                    _xp = value;
                    Skill.SetSkillExperience(_xp);
                    CheckForLevelUp();

                    LEEvents.RaiseEvent(new LEXPEventArgs
                    {
                        SkillName = Name,
                        ChangedXP = ChangedXP,
                        OldXP = oldxp,
                        NewXP = _xp,
                        CurrentLevel = Level
                    });
                }
            }

        }
        public int Level
        {
            get { return _level; }
            set
            {
                if (_level != value && value <= MaxLevel)
                {
                    value = Math.Max(0, Math.Min(MaxLevel, value));
                    ModEntry.Logger.LogDebug($"LESkill: {Skill.Type.Name}, set Level {_level} -> {value}");

                    _level = value;
                    Skill.SetSkillLevel(value);
                    int newxp = GetXPByLevel(Level);
                    int oldxp = _xp;
                    ChangedXP = newxp - oldxp;
                    _xp = newxp;
                    Skill.SetSkillExperience(_xp);
                    LEEvents.RaiseEvent(new LEXPEventArgs
                    {
                        SkillName = Name,
                        ChangedXP = ChangedXP,
                        OldXP = oldxp,
                        NewXP = newxp,
                        CurrentLevel = Level
                    });

                    ModEntry.Logger.LogDebug($"LESkill: {Name}, set Level {value}; xp: {XP}/{RequiredXPNextLevel}");
                }
            }
        }
        public int RequiredXPNextLevel
        {
            get
            {
                return GetRequiredXP(Level);
            }
        }
        public LESkill(ISkill skill, LEEvents le_events, double? needed_xp_factor = null, List<int> xp_table = null)
        {
            Skill = skill;
            LEEvents = le_events;
            args.SkillName = Skill.Type.Name;
            ItemCategories = new List<int>(Skill.ExtraItemCategories());

            ModEntry.Logger.LogDebug($"LESkill: {Name} old lvl: {Skill.GetSkillLevel()}; old xp: {Skill.GetSkillExperience()}");

            if (xp_table != null)
            {
                XPTable = xp_table;
            }
            if (needed_xp_factor != null)
            {
                NeededXPFactor = needed_xp_factor.Value;
            }
            GenerateXPTable(MaxLevel + 1);
            var oldxp = Skill.GetSkillExperience();
            var oldlevel = Skill.GetSkillLevel();
            _level = Math.Max(GetLevelByXP(oldxp), oldlevel);
            _xp = Math.Max(GetXPByLevel(oldlevel), oldxp);
            Skill.SetSkillExperience(_xp);
            CheckForLevelUp();

            ModEntry.Logger.LogDebug($"LESkill: {Name} lvl: {_level}; xp: {_xp}/{RequiredXPNextLevel}; xpt count: {XPTable.Count}");

            foreach(int lvl in new List<int> { 0, 1, 2, 3, 5, 9, 10, 11, 14, 15, 16, 20, 25, 26, 30, 40, 44, 45, 46, 50, 60, 70, 80, 90, 99, 100, 101 }) {
                ModEntry.Logger.LogDebug(String.Format("LESkill: XP Table: {0, 3} -> {1, 3} => {2, 10}", lvl, lvl+1, GetRequiredXP(lvl)));
            }
        }

        public int GetRequiredXP(int level)
        {
            if (level < XPTable.Count && level <= MaxLevel)
            {
                return XPTable[level];
            }
            return MaxXP;
        }

        public int GetXPByLevel(int level)
        {
            if (level == 0)
                return 0;

            if (level > 0 && level - 1 < XPTable.Count)
                return XPTable[level - 1];

            return MaxXP;
        }

        private void CheckForLevelUp()
        {
            int level = GetLevelByXP();

            ModEntry.Logger.LogDebug($"checkForLevelUp: old: {_level}, new: {level}");

            if (level >= 0 && level <= XPTable.Count && level <= MaxLevel && _level != level)
            {
                _level = level;
                Skill.SetSkillLevel(level);
                ModEntry.Logger.LogDebug($"checkForLevelUp: xp: {_xp}, lvl: {_level} -> {level}, xp_table: {XPTable[level]}");
            }
        }


        public int GetLevelByXP()
        {
            return GetLevelByXP(_xp);
        }
        private int GetLevelByXP(int xp)
        {
            if (XPTable.Count > 0 && xp > MaxXP)
                return XPTable.Count - 1;

            int ret = 0;
            for (int lvl = 0; lvl < XPTable.Count; lvl++)
            {
                if (xp >= XPTable[lvl])
                {
                    ret = lvl + 1;
                }
            }

            return ret;
        }
        private void GenerateXPTable(int maxlevel)
        {
            const int MIN_XP = 300;

            int xp = XPTable.Count > 0 ? XPTable[XPTable.Count - 1] : 0;
            for (int level = XPTable.Count; XPTable.Count < maxlevel; level++)
            {
                if (level > 50)
                    xp = xp + MIN_XP + (int)Math.Round((Math.Pow(level, 2) * level / 50.0) * NeededXPFactor);
                else
                    xp = xp + MIN_XP + (int)Math.Round((Math.Pow(level, 3) * 2 + 5000) * NeededXPFactor);

                XPTable.Add(xp);
            }

            MaxXP = XPTable.Max();

            ModEntry.Logger.LogDebug($"generateXPTable {maxlevel}; needed XP: {XPTable[0]}, {XPTable[1]}, ..., {XPTable[XPTable.Count / 2]}, ..., {XPTable[XPTable.Count - 1]}; max XP {MaxXP}");
        }

        private int _level = 0;
        private int _xp = 0;
        private readonly LEXPEventArgs args = new LEXPEventArgs();
    }
}
