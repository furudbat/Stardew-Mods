using System;

namespace LevelExtender.Logging
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>A wrapper for the Stardew Valley logger to simplify the interface and restrict what is logged.</summary>
    public static class Logger
    {
        /*********
        ** Public methods
        *********/
        public static void LogDebug(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Debug)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Debug);
        }

        public static void LogTrace(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Trace)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Trace);
        }
        public static void LogVerbose(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Verbose)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Trace);
        }

        public static void LogInformation(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Information)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Info);
        }

        public static void LogWarning(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Warning)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Warn);
        }

        public static void LogError(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Error)
                ModEntry.LogMonitor.Log(message.AddErrorText(), StardewModdingAPI.LogLevel.Error);
        }

        public static void LogCritical(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Critical)
                ModEntry.LogMonitor.Log(message.AddErrorText(), StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogCriticalWarning(string message)
        {
            if (ModEntry.Config.LogLevel >= LogLevel.Critical)
                ModEntry.LogMonitor.Log(message, StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogDisplay(string message)
        {
            ModEntry.LogMonitor.Log(message);
        }


        /*********
        ** Private methods
        *********/
        private static string AddErrorText(this string message)
        {
            return $"{message}{Environment.NewLine}Please file a bug report on NexusMods.";
        }
    }
}
