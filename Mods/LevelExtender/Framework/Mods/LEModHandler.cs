using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace LevelExtender.LEAPI
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>
    /// Register your skill mod with this class to add it to the system.
    /// </summary>
    static class LEModHandler
    {
        /// <summary>Whether the mod is initialised and ready to register skill mods.</summary>
        private static bool IsInitialised;

        /// <summary>The mods to add once the handler is initialised.</summary>
        private static readonly List<ISkillMod> PendingMods = new List<ISkillMod>();

        /// <summary>The registered mods.</summary>
        private static readonly List<ISkillMod> Mods = new List<ISkillMod>();

        /// <summary>Register a skill mod for the prestige system.</summary>
        /// <param name="mod">The mod you wish to register. the mod and its profession Ids cannot already exist in the system, 
        /// and the mod must implement ISkillMod. It is recommended to inherit from SkillMod class.</param>
        public static void RegisterMod(ISkillMod mod, IMonitor logMonitor)
        {
            if (IsInitialised)
                RegisterModImpl(mod, logMonitor);
            else
                PendingMods.Add(mod);
        }

        /// <summary>Initialise the mod handler and add any pending mods.</summary>
        internal static void Initialise(IMonitor logMonitor)
        {
            IsInitialised = true;

            foreach (ISkillMod mod in PendingMods)
                RegisterModImpl(mod, logMonitor);
            PendingMods.Clear();
        }

        /// <summary>Register a skill mod for the system.</summary>
        /// <param name="mod">The mod you wish to register. the mod and its profession Ids cannot already exist in the system, 
        /// and the mod must implement ISkillMod. It is recommended to inherit from SkillMod class.</param>
        public static void RegisterModImpl(ISkillMod mod, IMonitor LogMonitor)
        {
            if (!IsInitialised)
                throw new InvalidOperationException($"The mod handler is not ready to register skill mods yet.");

            if (!mod.IsFound)
            {
                LogMonitor.Log($"{mod.DisplayName} Mod not found. Mod not registered.", StardewModdingAPI.LogLevel.Info);
                return;
            }
            try
            {
                LogMonitor.Log($"Registering mod: {mod.DisplayName} ...", StardewModdingAPI.LogLevel.Info);
                if (Mods.Any(x => x.GetType() == mod.GetType()))
                {
                    LogMonitor.Log($"Cannot load mod: {mod.DisplayName}, as it is already loaded.", StardewModdingAPI.LogLevel.Warn);
                    return;
                }
                Mods.Add(mod);
                LogMonitor.Log($"Registered mod: {mod.DisplayName}", StardewModdingAPI.LogLevel.Info);
            }
            catch (Exception exception)
            {
                LogMonitor.Log($"Failed to register mod. please ensure mod implements the ISKillMod interface correctly and none of its members generate errors when called. {Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}", StardewModdingAPI.LogLevel.Warn);
            }
        }

        /// <summary>Get the skills added by other mods.</summary>
        public static IEnumerable<Skill> GetAddedSkills()
        {
            return Mods.Where(x => x.AdditionalSkills != null).SelectMany(x => x.AdditionalSkills);
        }
    }
}
