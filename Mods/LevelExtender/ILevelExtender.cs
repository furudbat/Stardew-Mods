using System;
using System.Collections.Generic;
using LevelExtender.Common;
using LevelExtender.LEAPI;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using StardewValley;

namespace LevelExtender
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
    }
}
