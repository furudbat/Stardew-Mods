using System;

namespace LevelExtender.LEAPI
{
    public class LEXPEventArgs : EventArgs
    {
        public string SkillName { get; set; }
    }
    public class LEEvents
    {
        public event EventHandler<LEXPEventArgs> OnXPChanged;

        public void RaiseEvent(LEXPEventArgs args)
        {
            if (OnXPChanged != null)
            {
                { OnXPChanged(this, args); }
            }
        }
    }
}
