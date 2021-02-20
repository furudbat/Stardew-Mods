using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.LEAPI;
using LevelExtender.Logging;

namespace LevelExtender.Common
{
    class LESkill : ILESkill
    {
        public readonly int MaxLevel = 100;

        public readonly Skill Skill;
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
        public LESkill(Skill skill, LEEvents le_events, double? needed_xp_factor = null, List<int> xp_table = null)
        {
            Skill = skill;
            LEEvents = le_events;
            args.SkillName = Skill.Type.Name;
            ItemCategories = new List<int>(Skill.ExtraItemCategories());

            if (xp_table != null)
            {
                XPTable = xp_table;
            }
            if (needed_xp_factor != null)
            {
                NeededXPFactor = needed_xp_factor.Value;
            }
            GenerateXPTable(MaxLevel+1);
            _level = Skill.GetSkillLevel();
            _xp = skill.GetSkillExperience();
            Skill.SetSkillExperience(_xp);
            CheckForLevelUp();

            ModEntry.Logger.LogDebug($"LESkill: {Name} lvl: {_level}; xp: {_xp}/{RequiredXPNextLevel}; xpt count: {XPTable.Count}");
            ModEntry.Logger.LogVerbose($"LESkill: XP Table: {XPTable}");
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
            if (XPTable.Count > 0 && _xp > MaxXP)
                return XPTable.Count - 1;

            int ret = 0;
            for (int lvl = 0; lvl < XPTable.Count; lvl++)
            {
                if (_xp >= XPTable[lvl])
                {
                    ret = lvl+1;
                }
            }

            return ret;
        }
        private void GenerateXPTable(int level)
        {
            const int MIN_XP = 300;

            int xp = (XPTable.Count > 0) ? XPTable[XPTable.Count - 1] : 0;
            for (int lvl = XPTable.Count; XPTable.Count < level; lvl++)
            {
                double base_xp = (lvl < 45) ? 1000.0 : (Math.Pow(lvl, 2) / 2);
                xp += MIN_XP + (int)Math.Round(base_xp * lvl * NeededXPFactor);
                XPTable.Add(xp);
            }

            MaxXP = (MaxLevel - 1 < XPTable.Count) ? XPTable[MaxLevel-1] : XPTable.Max();

            ModEntry.Logger.LogDebug($"generateXPTable {level}; needed XP: {XPTable[0]}, {XPTable[1]}, ..., {XPTable[XPTable.Count / 2]}, ..., {XPTable[XPTable.Count - 1]}; max XP {MaxXP}");
        }

        private int _level = 0;
        private int _xp = 0;
        private readonly LEXPEventArgs args = new LEXPEventArgs();
    }
}
