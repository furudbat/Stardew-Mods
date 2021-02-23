using System;
using StardewModdingAPI;

namespace LevelExtender.Logging
{
    internal class Logger
    {
        public ModConfig Config { get; private set; }
        public IMonitor LogMonitor { get; private set; }
        public Logger(ModConfig config, IMonitor logMonitor)
        {
            Config = config;
            LogMonitor = logMonitor;
        }
        public void Reset(ModConfig config, IMonitor logMonitor)
        {
            Config = config;
            LogMonitor = logMonitor;
        }
        public void LogDebug(string message)
        {
            if (Config.LogLevel >= LogLevel.Debug)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Debug);
        }

        public void LogTrace(string message)
        {
            if (Config.LogLevel >= LogLevel.Trace)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Trace);
        }
        public void LogVerbose(string message)
        {
            if (Config.LogLevel >= LogLevel.Verbose)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Trace);
        }

        public void LogInformation(string message)
        {
            if (Config.LogLevel >= LogLevel.Information)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Info);
        }

        public void LogWarning(string message)
        {
            if (Config.LogLevel >= LogLevel.Warning)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Warn);
        }

        public void LogError(string message)
        {
            if (Config.LogLevel >= LogLevel.Error)
                LogMonitor.Log(AddErrorText(message), StardewModdingAPI.LogLevel.Error);
        }

        public void LogCritical(string message)
        {
            if (Config.LogLevel >= LogLevel.Critical)
                LogMonitor.Log(AddErrorText(message), StardewModdingAPI.LogLevel.Alert);
        }

        public void LogCriticalWarning(string message)
        {
            if (Config.LogLevel >= LogLevel.Critical)
                LogMonitor.Log(message, StardewModdingAPI.LogLevel.Alert);
        }

        public void LogDisplay(string message)
        {
            LogMonitor.Log(message);
        }

        private string AddErrorText(string message)
        {
            return $"{message}{Environment.NewLine}Please file a bug report on NexusMods.";
        }
    }
}
