using LevelExtender.Logging;
using StardewValley;

namespace LevelExtender.Framework.Professions
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Special handling for the defender profession, which adds 25 to the player's maximum health.</summary>
    internal class DefenderSpecialHandling : IProfessionSpecialHandling
    {
        /*********
        ** Public methods
        *********/
        public void ApplyEffect()
        {
            Logger.LogInformation("Applying defender effect.");
            Game1.player.maxHealth += 25;
        }

        public void RemoveEffect()
        {
            Logger.LogInformation("Removing defender effect.");
            Game1.player.maxHealth -= 25;
        }
    }
}
