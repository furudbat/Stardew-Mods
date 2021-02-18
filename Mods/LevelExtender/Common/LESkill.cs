﻿using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.LEAPI;
using LevelExtender.Logging;

namespace LevelExtender.Common
{
    class LESkill : ILESkill
    {
        public readonly int MAX_LVL = 100;

        public readonly Skill Skill;
        public LEEvents LEEvents { get; private set; }
        public bool LevelByXp { get; private set; } = false;
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
                    ChangedXP = value - _xp;
                    _xp = value;
                    CheckForLevelUp();
                    LEEvents.RaiseEvent(args);

                    ModEntry.Logger.LogDebug($"LESkill: {Name}, set XP {value} (diff {ChangedXP})");
                }
            }

        }
        public int Level
        {
            get { return _level; }
            set
            {

                ModEntry.Logger.LogDebug($"LESkill: {Skill.Type.Name}, set Level {_level} -> {value}");

                _level = value;
                Skill.SetSkillLevel(value);

                if (!LevelByXp)  {
                    int newxp = GetXPByLevel(Level);
                    Skill.SetSkillExperience(newxp);
                    _xp = newxp;
                    ModEntry.Logger.LogDebug($"LESkill: {Name}, set XP by Level; newxp: {newxp}");
                }
                LevelByXp = false;

                ModEntry.Logger.LogDebug($"LESkill: {Name}, set Level {value}; xp: {XP}/{RequiredXPNextLevel}");
            }
        }
        public int RequiredXPNextLevel
        {
            get {
                return GetRequiredXP(Level);
            }
        }
        public LESkill(Skill skill, LEEvents le_events, double? needed_xp_factor = null, List<int> xp_table = null)
        {
            Skill = skill;
            LEEvents = le_events;
            args.SkillName = Skill.Type.Name;
            ItemCategories = new List<int> (Skill.ExtraItemCategories());

            if (xp_table != null) {
                XPTable = xp_table;
            }
            if (needed_xp_factor != null)
            {
                NeededXPFactor = needed_xp_factor.Value;
            }

            _level = skill.GetSkillLevel();
            _xp = skill.GetSkillExperience();

            GenerateXPTable(MAX_LVL);
            if (XPTable != null && XPTable.Count > 0)
            {
                MaxXP = XPTable.Max();
                ModEntry.Logger.LogDebug($"LESkill: {Name}, max XP {MaxXP}");
            }

            CheckForLevelUp();

            ModEntry.Logger.LogDebug($"LESkill: {Name} lvl: {_level}; xp: {_xp}/{RequiredXPNextLevel}; xpt count: {XPTable.Count}");
            ModEntry.Logger.LogVerbose($"LESkill: XP Table: {XPTable}");
        }

        public int GetRequiredXP(int level)
        {
            if (level < XPTable.Count)
                return XPTable[level];
            else
                GenerateXPTable(level);

            return (level < XPTable.Count) ? XPTable[level] : -1;
        }

        public int GetXPByLevel(int level)
        {
            if (level <= 0)
                return 0;
            if (level-1 < XPTable.Count)
                return XPTable[level-1];

            return MaxXP;
        }

        private void CheckForLevelUp()
        {
            int level = GetLevelByXP();

            if (_level != level)
            {
                ModEntry.Logger.LogDebug($"checkForLevelUp: xp: {_xp}, lvl: {_level} -> {level}, xp_table: {XPTable[level]} -> {XPTable[_level]}");
                LevelByXp = true;
                Level = level;
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
                    ret = lvl;
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

            MaxXP = XPTable.Max();

            ModEntry.Logger.LogDebug($"generateXPTable {level}; needed XP: {XPTable[0]}, {XPTable[1]}, ..., {XPTable[XPTable.Count/2]}, ..., {XPTable[XPTable.Count-1]} ");
        }

        private int _level = 0;
        private int _xp = 0;
        private readonly LEXPEventArgs args = new LEXPEventArgs();
    }
}