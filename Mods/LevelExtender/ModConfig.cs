using System;
using System.Collections.Generic;
using LevelExtender.Logging;

namespace LevelExtender
{
    /// <summary>Represents options for this mod.</summary>
    [Serializable]
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The logging verbosity for the mod. A log level set to Verbose will log all entries.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        /// <summary>Whether testing mode is enabled, which adds testing specific commands to the system.</summary>
        public bool TestingMode { get; set; }

        public bool drawBars { get; set; } = true;

        public bool drawExtraItemNotifications { get; set; } = true;

        public int minItemPriceForNotifications { get; set; } = 50;

        //public List<string> skills { get; set; } = new List<string>();
        public bool DrawNotificationsAsHUDMessage { get; set; } = false;

        public bool CropsGrow { get; set; } = true;
        public bool MoreEXPByOneHitKills { get; set; } = true;
        public bool DropExtraItems { get; set; } = true;
        public bool DropExtraItemMessageExtras { get; set; } = true;

        public bool OverworldMonsters { get; set; } = false;
        public double OverworldMonstersSpawnRate { get; set; } = -1;
    }
}
