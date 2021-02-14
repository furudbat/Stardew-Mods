using System;
using LevelExtender.Framework.SkillTypes;

namespace LevelExtender.Common
{
    public class EXPEventArgs : EventArgs
    {
        public SkillType SkillType { get; set; }
    }
    public class LEEvents
    {
        public event EventHandler<EXPEventArgs> OnXPChanged;

        public void RaiseEvent(EXPEventArgs args)
        {
            if (OnXPChanged != null)
            {
                { OnXPChanged(this, args); }
            }
        }
    }
}
