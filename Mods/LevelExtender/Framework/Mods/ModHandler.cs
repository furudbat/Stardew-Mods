﻿using System;
using System.Collections.Generic;
using System.Linq;
using LevelExtender.Framework;
using LevelExtender.Logging;

namespace LevelExtender.Framework.Mods
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>
    /// Register your skill mod with this class to add it to the system.
    /// </summary>
    public static class ModHandler
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
        public static void RegisterMod(ISkillMod mod)
        {
            if (IsInitialised)
                RegisterModImpl(mod);
            else
                PendingMods.Add(mod);
        }

        /// <summary>Initialise the mod handler and add any pending mods.</summary>
        internal static void Initialise()
        {
            IsInitialised = true;

            foreach (ISkillMod mod in PendingMods)
                RegisterModImpl(mod);
            PendingMods.Clear();
        }

        /// <summary>Register a skill mod for the system.</summary>
        /// <param name="mod">The mod you wish to register. the mod and its profession Ids cannot already exist in the system, 
        /// and the mod must implement ISkillMod. It is recommended to inherit from SkillMod class.</param>
        public static void RegisterModImpl(ISkillMod mod)
        {
            if (!IsInitialised)
                throw new InvalidOperationException($"The mod handler is not ready to register skill mods yet.");

            if (!mod.IsFound)
            {
                Logger.LogInformation($"{mod.DisplayName} Mod not found. Mod not registered.");
                return;
            }
            try
            {
                Logger.LogInformation($"Registering mod: {mod.DisplayName} ...");
                if (Mods.Any(x => x.GetType() == mod.GetType()))
                {
                    Logger.LogWarning($"Cannot load mod: {mod.DisplayName}, as it is already loaded.");
                    return;
                }
                var intersectingMods = GetIntersectingModProfessions(mod);
                if (intersectingMods.Any())
                {
                    Logger.LogWarning($"Cannot load skill mod: {mod.DisplayName}, as it collides with another mod's skills. Details:");
                    foreach (var intersectingMod in intersectingMods)
                        Logger.LogWarning($"Skill mod {mod.DisplayName} registration failed due to {intersectingMod.Key.DisplayName}, for profession ids: {string.Join(",", intersectingMod.Value)}");
                    return;
                }
                Mods.Add(mod);
                Logger.LogInformation($"Registered mod: {mod.DisplayName}");
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Failed to register mod. please ensure mod implements the ISKillMod interface correctly and none of its members generate errors when called. {Environment.NewLine}{exception.Message}{Environment.NewLine}{exception.StackTrace}");
            }
        }

        /// <summary>Get the skills added by other mods.</summary>
        public static IEnumerable<Skill> GetAddedSkills()
        {
            return Mods.Where(x => x.AdditionalSkills != null).SelectMany(x => x.AdditionalSkills);
        }


        /// <summary>Get the mods and profession IDs which collide with an already-registered professions ID.</summary>
        /// <param name="mod">The mod to check.</param>
        private static IDictionary<ISkillMod, IEnumerable<int>> GetIntersectingModProfessions(ISkillMod mod)
        {
            var intersectingMods = new Dictionary<ISkillMod, IEnumerable<int>>();
            Logger.LogInformation($"Loaded mods: {Mods.Count}");
            foreach (var loadedMod in Mods)
            {
                var loadedModProfessions = loadedMod.AdditionalSkills?.SelectMany(x => x.GetAllProfessionIds());
                if (loadedModProfessions == null)
                    continue;
                var modProfessions = mod.AdditionalSkills.SelectMany(x => x.GetAllProfessionIds());
                var intersectingProfessions = loadedModProfessions.Intersect(modProfessions).ToList();
                Logger.LogInformation($"intersecting profession for {loadedMod.DisplayName} and {mod.DisplayName}: {intersectingProfessions.Count}");
                if (intersectingProfessions.Any())
                    intersectingMods.Add(loadedMod, intersectingProfessions);
            }
            return intersectingMods;
        }
    }
}
