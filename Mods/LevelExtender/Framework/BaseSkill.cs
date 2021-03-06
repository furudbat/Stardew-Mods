using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace LevelExtender.Framework
{
    abstract class BaseSkill : ISkill
    {
        public readonly int MaxLevel = 100;
        public List<int> XPTable { get; protected set; } = new List<int>();
        public double NeededXPFactor { get; set; } = 1.0;
        public int MaxXP { get; protected set; } = 0;
        public List<int> ItemCategories { get; set; } = new List<int>();

        public int GetRequiredXPNextLevel(Farmer farmer)
        {
            return GetRequiredXP(GetSkillLevel(farmer));
        }

        public abstract string Id { get; }
        public int[] ExperienceCurve { get => XPTable.ToArray(); set => XPTable = value.ToList(); }

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

        public int GetLevelByXP(Farmer farmer)
        {
            return GetLevelByXP(GetExperienceFor(farmer));
        }
        protected int GetLevelByXP(int xp)
        {
            if (XPTable.Count > 0 && xp >= MaxXP)
                return MaxLevel;

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
        protected void GenerateXPTable(int maxlevel)
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

        public abstract string GetName();
        public abstract int GetExperienceFor(Farmer farmer);
        public abstract int GetSkillLevel(Farmer farmer);
    }
}
