using System;
using Harmony;
using LevelExtender.Common;
using StardewValley;

namespace LevelExtender.Patches
{
    [HarmonyPatch(typeof(Farmer))]
    [HarmonyPatch(nameof(Farmer.addItemToInventoryBool))]
    [HarmonyPatch(new Type[] { typeof(Item), typeof(bool) })]
    class FarmerPatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            FarmerPatch.mod = mod;
        }

        public static bool Prefix(Item item, bool makeActiveObject, ref bool __result)
        {
            try
            {
                return mod.addItemToInventoryBool_Prefix(item, makeActiveObject);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(Prefix)}:\n{ex}");
            }
            return true;
        }
    }
}
