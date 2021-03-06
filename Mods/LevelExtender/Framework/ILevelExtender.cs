using System;
using System.Collections.Generic;
using LevelExtender.Framework.ItemBonus;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace LevelExtender.Framework
{
    interface ILevelExtender : Api
    {
        Logger Logger { get; }

        bool SetNeededXPFactor(string name, double value);

        List<BaseSkill> Skills { get; }
        List<LEVanillaSkill> VanillaSkills { get; }
        ModConfig EditConfig(Action<ModConfig> func);

        bool SetVanillaLevel(string skillId, int value);
        bool SetVanillaXP(string skillId, int value);

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
        void plant_Postfix(HoeDirt hoeDirt, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location);
        bool addItemToInventoryBool_Prefix(Item item, bool makeActiveObject);
        void sellToStorePrice_Postfix(StardewValley.Object item, long specificPlayerID, ref int newprice);
    }
}
