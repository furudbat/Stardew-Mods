using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelExtender.Common;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LevelExtender.Patches
{
    class FarmerPatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            FarmerPatch.mod = mod;
        }

        public static bool addItemToInventoryBool_Prefix(Item item, bool makeActiveObject)
        {
            try
            {
                return mod.addItemToInventoryBool_Prefix(item, makeActiveObject);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(addItemToInventoryBool_Prefix)}:\n{ex}");
                return true; // run original logic
            }
        }
    }
}
