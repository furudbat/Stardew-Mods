using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace LevelExtender.Framework
{
    class LEVanillaSkill : BaseSkill
    {
        public readonly VanillaSkill Skill;
        public LEEvents LEEvents { get; private set; }
        public bool SetXPByLevel { get; private set; } = false;
        public int ChangedXP { get; private set; } = 0;

        public string Name
        {
            get
            {
                return Skill.Name;
            }
        }

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
                    ModEntry.Logger.LogDebug($"LESkill: {Skill.Name}, set Level {_level} -> {value}");

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

                    ModEntry.Logger.LogDebug($"LESkill: {Name}, set Level {value}; xp: {XP}/{GetRequiredXPNextLevel(farmer)}");
                }
            }
        }


        public LEVanillaSkill(Farmer farmer, VanillaSkill skill, LEEvents le_events, double? needed_xp_factor = null, List<int> xp_table = null)
        {
            this.farmer = farmer;
            Skill = skill;
            LEEvents = le_events;
            args.SkillName = Skill.Name;
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

            ModEntry.Logger.LogDebug($"LESkill: {Name} lvl: {_level}; xp: {_xp}/{GetRequiredXPNextLevel(farmer)}; xpt count: {XPTable.Count}");

            foreach(int lvl in new List<int> { 0, 1, 2, 3, 5, 9, 10, 11, 14, 15, 16, 20, 25, 26, 30, 40, 44, 45, 46, 50, 60, 70, 80, 90, 99, 100, 101 }) {
                ModEntry.Logger.LogDebug(String.Format("LESkill: XP Table: {0, 3} -> {1, 3} => {2, 10}", lvl, lvl+1, GetRequiredXP(lvl)));
            }
        }

        private void CheckForLevelUp()
        {
            int level = GetLevelByXP(farmer);

            ModEntry.Logger.LogDebug($"checkForLevelUp: old: {_level}, new: {level}");

            if (level >= 0 && level <= XPTable.Count && level <= MaxLevel && _level != level)
            {
                _level = level;
                Skill.SetSkillLevel(level);
                ModEntry.Logger.LogDebug($"checkForLevelUp: xp: {_xp}, lvl: {_level} -> {level}, xp_table: {XPTable[level]}");
            }
        }

        public override string Id => Name;
        public override string GetName() => Name;
        public override int GetExperienceFor(Farmer farmer) => XP;
        public override int GetSkillLevel(Farmer farmer) => Level;

        private int _level = 0;
        private int _xp = 0;
        private readonly LEXPEventArgs args = new LEXPEventArgs();
        private readonly Farmer farmer;
    }
}
