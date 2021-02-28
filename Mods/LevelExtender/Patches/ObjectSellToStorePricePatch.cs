using System;
using Harmony;
using LevelExtender.Framework;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Patches
{
    [HarmonyPatch(typeof(StardewValley.Object))]
    [HarmonyPatch(nameof(StardewValley.Object.sellToStorePrice))]
    [HarmonyPatch(new Type[] { typeof(int) })]
    class ObjectSellToStorePricePatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            ObjectSellToStorePricePatch.mod = mod;
        }
        public static void Postfix(StardewValley.Object __instance, long specificPlayerID, ref int __result)
        {
            try
            {
                mod.sellToStorePrice_Postfix(__instance, specificPlayerID, ref __result);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(Postfix)}:\n{ex}");
            }
        }
    }
}
