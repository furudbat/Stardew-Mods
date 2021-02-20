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
    class GameLocationPatch
    {
        private static ILevelExtender mod;

        // call this method from your Entry class
        public static void Initialize(ILevelExtender mod)
        {
            GameLocationPatch.mod = mod;
        }

        public static bool damageMonster_Prefix(
          Rectangle areaOfEffect,
          int minDamage,
          int maxDamage,
          bool isBomb,
          float knockBackModifier,
          int addedPrecision,
          float critChance,
          float critMultiplier,
          bool triggerMonsterInvincibleTimer,
          Farmer who)
        {
            try
            {
                return mod.damageMonster_Prefix(areaOfEffect, minDamage, maxDamage, isBomb, knockBackModifier, addedPrecision, critChance, critMultiplier, triggerMonsterInvincibleTimer, who);
            }
            catch (Exception ex)
            {
                mod.Logger.LogError($"Failed in {nameof(damageMonster_Prefix)}:\n{ex}");
                return true; // run original logic
            }
        }
    }
}
