using System.Collections.Generic;
using LevelExtender.Logging;

namespace LevelExtender.Framework.Professions.Registration
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    // ReSharper disable once UnusedMember.Global - created through reflection.
    public sealed class MiningRegistration : ProfessionRegistration
    {
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Mining professions...");
            Miner = new TierOneProfession
            {
                Id = 18

            };
            Geologist = new TierOneProfession
            {
                Id = 19

            };
            Blacksmith = new TierTwoProfession
            {
                Id = 20,
                TierOneProfession = Miner
            };
            Prospector = new TierTwoProfession
            {
                Id = 21,
                TierOneProfession = Miner
            };
            Excavator = new TierTwoProfession
            {
                Id = 22,
                TierOneProfession = Geologist
            };
            Gemologist = new TierTwoProfession
            {
                Id = 23,
                TierOneProfession = Geologist
            };
            Miner.TierTwoProfessions = new List<TierTwoProfession>
            {
                Blacksmith,
                Prospector
            };
            Geologist.TierTwoProfessions = new List<TierTwoProfession>
            {
                Excavator,
                Gemologist
            };
            Logger.LogInformation("Mining professions registered.");
        }
    }
}
