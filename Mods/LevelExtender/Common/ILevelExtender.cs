using System;
using System.Collections.Generic;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LevelExtender.Common
{
    internal interface ILevelExtender
    {
        Logger Logger { get; }

        bool SetLevel(string name, int value);
        bool SetXP(string name, int value);
        bool SetNeededXPFactor(string name, double value);

        List<LESkill> Skills { get; }
        ModConfig EditConfig(Action<ModConfig> func);

        bool damageMonster_Prefix(
          GameLocation currentLocation,
          Rectangle areaOfEffect,
          int minDamage,
          int maxDamage,
          bool isBomb,
          float knockBackModifier,
          int addedPrecision,
          float critChance,
          float critMultiplier,
          bool triggerMonsterInvincibleTimer,
          Farmer who);
        bool addItemToInventoryBool_Prefix(Item item, bool makeActiveObject);
        void sellToStorePrice_Postfix(StardewValley.Object item, long specificPlayerID, ref int newprice);
    }
}
