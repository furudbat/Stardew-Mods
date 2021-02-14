namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Represents special handling for professions where Stardew Valley applies the profession's effects in a custom manner.</summary>
    public interface IProfessionSpecialHandling
    {
        /*********
        ** Methods
        *********/
        /// <summary>Apply effects for the profession.</summary>
        void ApplyEffect();

        /// <summary>Remove effects for the profession.</summary>
        void RemoveEffect();
    }
}
