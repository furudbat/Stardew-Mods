using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.Framework;
using LevelExtender.Framework.SkillTypes;
using LevelExtender.Logging;

namespace LevelExtender.Common
{
    public class LESkill
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
                    checkForLevelUp();
                    LEEvents.RaiseEvent(args);

                    Logger.LogDebug($"LESkill: {Name}, set XP {value} (diff {ChangedXP})");
                }
            }

        }
        public int Level
        {
            get { return _level; }
            set
            {

                Logger.LogDebug($"LESkill: {Skill.Type.Name}, set Level {_level} -> {value}");

                _level = value;
                Skill.SetSkillLevel(value);

                if (!LevelByXp)  {
                    int reqxp = getRequiredXP(Level);
                    Skill.SetSkillExperience(reqxp);
                    _xp = reqxp;
                    Logger.LogDebug($"LESkill: {Name}, set XP by Level; xp: {_xp}");
                }
                LevelByXp = false;

                Logger.LogDebug($"LESkill: {Name}, set Level {value}; xp: {XP}/{RequiredXPNextLevel}");
            }
        }
        public int RequiredXPNextLevel
        {
            get {
                return getRequiredXP(Level);
            }
        }
        public LESkill(Skill skill, LEEvents le_events, int xp, double? needed_xp_factor = null, List<int> xp_table = null, List<int> item_categories = null)
        {

            Skill = skill;
            LEEvents = le_events;
            args.SkillType = Skill.Type;
            if (xp_table != null) {
                XPTable = xp_table;
            }

            if (needed_xp_factor != null)
            {
                NeededXPFactor = needed_xp_factor.Value;
            }
            if (item_categories != null)
            {
                ItemCategories = item_categories;
            }

            Skill.SetSkillExperience(xp);
            _level = skill.GetSkillLevel();
            _xp = xp;

            generateXPTable(MAX_LVL);
            if (XPTable != null && XPTable.Count > 0)
            {
                MaxXP = XPTable.Max();
                Logger.LogDebug($"LESkill: {Name}, max XP {MaxXP}");
            }

            checkForLevelUp();

            Logger.LogDebug($"LESkill: {Name} lvl: {_level}; xp: {_xp}/{RequiredXPNextLevel}; xpt count: {XPTable.Count}");
            Logger.LogVerbose($"LESkill: XP Table: {XPTable}");
        }

        public int getRequiredXP(int level)
        {
            if (level < XPTable.Count)
                return XPTable[level];
            else
                generateXPTable(level);

            return (level < XPTable.Count) ? XPTable[level] : -1;
        }

        public int getXPByLevel(int level)
        {
            if (level <= 0)
                return 0;
            if (level < XPTable.Count)
                return XPTable[level];

            return MaxXP;
        }

        private void checkForLevelUp()
        {
            int level = getLevelByXP();

            if (level != _level)
            {
                Logger.LogDebug($"checkForLevelUp: {_level} -> {level}");
                LevelByXp = true;
                _level = level;
            }
        }


        public int getLevelByXP()
        {
            if (XPTable.Count > 0 && _xp > MaxXP) 
                return XPTable.Count - 1;

            for (int lvl = 0; lvl < XPTable.Count; lvl++)
            {
                if (_xp < XPTable[lvl])
                {
                    return lvl;
                }
            }

            return 0;
        }
        private void generateXPTable(int level)
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

            Logger.LogDebug($"generateXPTable {level}; needed XP: {XPTable[0]}, {XPTable[1]}, ..., {XPTable[XPTable.Count/2]}, ..., {XPTable[XPTable.Count-1]} ");
        }

        private int _level = 0;
        private int _xp = 0;
        private readonly EXPEventArgs args = new EXPEventArgs();
    }
}
