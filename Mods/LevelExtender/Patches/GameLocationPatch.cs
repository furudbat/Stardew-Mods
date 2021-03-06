using System;
using Harmony;
using LevelExtender.Framework;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LevelExtender.Patches
{
    [HarmonyPatch(typeof(GameLocation))]
    [HarmonyPatch(nameof(GameLocation.damageMonster))]
    [HarmonyPatch(new Type[] { typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) })]
    class GameLocationPatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            GameLocationPatch.mod = mod;
        }

        public static bool Prefix(
          GameLocation __instance,
          Rectangle areaOfEffect,
          int minDamage,
          int maxDamage,
          bool isBomb,
          float knockBackModifier,
          int addedPrecision,
          float critChance,
          float critMultiplier,
          bool triggerMonsterInvincibleTimer,
          Farmer who,
          ref bool __result
          )
        {
            try
            {
                return mod.damageMonster_Prefix(__instance, areaOfEffect, minDamage, maxDamage, isBomb, knockBackModifier, addedPrecision, critChance, critMultiplier, triggerMonsterInvincibleTimer, who);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(Prefix)}:\n{ex}");
            }
            return true;
        }
    }
}
