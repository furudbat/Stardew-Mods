using System;
using Harmony;
using LevelExtender.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Patches
{
    [HarmonyPatch(typeof(HoeDirt))]
    [HarmonyPatch(nameof(HoeDirt.plant))]
    [HarmonyPatch(new Type[] { typeof(int), typeof(int), typeof(int), typeof(Farmer), typeof(bool), typeof(GameLocation) })]
    class HoeDirtPlantPatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            HoeDirtPlantPatch.mod = mod;
        }
        public static void Postfix(HoeDirt __instance, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location, ref bool __result)
        {
            try
            {
                mod.plant_Postfix(__instance, index, tileX, tileY, who, isFertilizer, location);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(Postfix)}:\n{ex}");
            }
        }
    }
}
