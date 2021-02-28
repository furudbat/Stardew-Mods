using System;
using LevelExtender.Logging;

namespace LevelExtender
{
    public enum CropsGrowOption {
        LinearByLevel,
        RandomByLevel
    }

    public enum DrawExtraItemNotificationsOption {
        Disable,
        Enable,
        EnableWithAmount
    }
    /// <summary>Represents options for this mod.</summary>
    [Serializable]
    public class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The logging verbosity for the mod. A log level set to Verbose will log all entries.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>Whether testing mode is enabled, which adds testing specific commands to the system.</summary>
        public bool TestingMode { get; set; } = false;

        public bool DrawXPBars { get; set; } = true;
        public bool DrawXPGain { get; set; } = true;

        public DrawExtraItemNotificationsOption DrawExtraItemNotifications { get; set; } = DrawExtraItemNotificationsOption.Disable;
        public int MinItemPriceForNotifications { get; set; } = 50;
        public bool DrawNotificationsAsHUDMessage { get; set; } = false;

        public CropsGrowOption CropsGrow { get; set; } = CropsGrowOption.LinearByLevel;
        public bool MoreEXPByOneHitKills { get; set; } = true;
        public bool DropExtraItemsByLevel { get; set; } = true;
        public bool DropExtraItemsByProfession { get; set; } = true;

        public bool OverworldMonsters { get; set; } = false;
        public double OverworldMonstersSpawnRate { get; set; } = -1;
        public bool FishingOverhaul { get; set; } = true;
        public bool BetterItemQuality { get; set; } = true;
    }
}
