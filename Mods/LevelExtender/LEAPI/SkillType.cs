using System;
using System.Linq;

namespace LevelExtender.LEAPI
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Represents a skill type in Stardew Valley (e.g. Farming, Fishing, Foraging).</summary>
    [Serializable]
    public class SkillType
    {
        /*********
        ** Accessors
        *********/
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - setter used by deserializer.
        public string Name { get; private set; }

        /// <summary>The ordinal and lookup used to get the skill type from Stardew Valley.</summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - setter used by deserializer.
        public int Ordinal { get; private set; }


        /*********
        ** Public methods
        *********/
        public SkillType() { }

        // ReSharper disable once MemberCanBeProtected.Global - this time resharper is just out of it's gourd. this is used publically.
        public SkillType(string name, int ordinal)
        {
            this.Name = name;
            this.Ordinal = ordinal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == this.GetType() && this.Equals((SkillType)obj);
        }

        public bool Equals(SkillType other)
        {
            return string.Equals(this.Name, other.Name) && this.Ordinal == other.Ordinal;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode -- used by deserializer only
                return ((this.Name?.GetHashCode() ?? 0) * 397) ^ this.Ordinal;
            }
        }

        public static bool operator ==(SkillType left, SkillType right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if ((object)left == null || (object)right == null)
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(SkillType left, SkillType right)
        {
            return !(left == right);
        }
    }
}

