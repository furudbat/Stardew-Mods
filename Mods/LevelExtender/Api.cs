using System;
using System.Collections.Generic;

namespace LevelExtender
{
    [Serializable]
    public class ItemBonusValue
    {
        public double Chance { get; set; } = 1.0;
        public int Value { get; set; } = 0;
    }

    [Serializable]
    public class ItemBonusBySkillData
    {
        public string SkillId { get; set; }
        public int Profession { get; set; } = -1;
        public int? BonusType { get; set; } = null;
        public List<int> ItemCategories { get; set; } = new List<int>();
        public List<string> Items { get; set; } = new List<string>();
        public List<string> ExtraItems { get; set; } = new List<string>();
        public int MinLevel { get; set; } = 0;
        public List<ItemBonusValue> Values { get; set; } = new List<ItemBonusValue> { new ItemBonusValue() };

        public double Chance
        {
            get
            {
                return (Values.Count > 0) ? Values[0].Chance : 1.0;
            }
            set
            {
                if (Values.Count == 0)
                {
                    Values.Add(new ItemBonusValue { Chance = value });
                }
                else
                {
                    Values[0].Chance = value;
                }
                Values.Sort((a, b) => b.Chance.CompareTo(a.Chance));
            }
        }
        public int Value
        {
            get
            {
                return (Values.Count > 0) ? Values[0].Value : 0;
            }
            set
            {
                if (Values.Count == 0)
                {
                    Values.Add(new ItemBonusValue { Value = value });
                }
                else
                {
                    Values[0].Value = value;
                }
            }
        }
    }

    [Serializable]
    public class LEXPEventArgs : EventArgs
    {
        public string SkillName { get; set; } = "";
        public int ChangedXP { get; set; } = 0;
        public int OldXP { get; set; } = 0;
        public int NewXP { get; set; } = 0;
        public int CurrentLevel { get; set; } = 0;
    }
    public class LEEvents
    {
        public event EventHandler<LEXPEventArgs> OnXPChanged;

        public void RaiseEvent(LEXPEventArgs args)
        {
            OnXPChanged?.Invoke(this, args);
        }
    }

    public interface Api
    {
        // old legacy Api
        void Spawn_Rate(double osr);
        int[] currentXP();
        int[] requiredXP();

        event EventHandler<LEXPEventArgs> OnXPChanged;

        // new skill api (works with SpaceCore)
        int GetSkillCurrentXP(string skillName);
        int GetSkillRequiredXP(string skillName);
        int GetSkillLevel(string skillName);
        int GetSkillMaxLevel(string skillName);
        int GetXPRequiredToLevel(string skillName, int level);

        // LE features
        void SetItemCategories(string skillName, IEnumerable<int> itemCategories);
        void RegisterItemBonuses(IEnumerable<ItemBonusBySkillData> values);
    }
}
