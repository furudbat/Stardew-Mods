using LevelExtender.Logging;
using StardewValley;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Special handling for the fighter profession, which adds 15 to the player's maximum health.</summary>
    internal class FighterSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Public methods
        *********/
        public void ApplyEffect()
        {
            Logger.LogInformation("Applying fighter effect.");
            Game1.player.maxHealth += 15;
        }

        public void RemoveEffect()
        {
            Logger.LogInformation("Removing fighter effect.");
            Game1.player.maxHealth -= 15;
        }
    }
}
