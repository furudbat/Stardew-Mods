using System;

namespace LevelExtender.LEAPI
{
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
}
