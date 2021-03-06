
# Contents
- [Contents](#contents)
  - [LevelExtender](#levelextender)
  - [Install](#install)
  - [Usage](#usage)
  - [Config](#config)
  - [Compatibility](#compatibility)
    - [Known Bugs](#known-bugs)
  - [Commands](#commands)
  - [Versions](#versions)
    - [2.0.0-unofficial.1-furudbat](#200-unofficial1-furudbat)
    - [1.5.3-v001](#153-v001)
    - [1.3.11](#1311)
  - [Credits](#credits)
 --------------------------------------------------------

## LevelExtender

[**Level Extender**](https://www.nexusmods.com/stardewvalley/mods/1471) is a [Stardew Valley](http://stardewvalley.net/) mod which extends the level cap to 100.

_I rewrite this for my own playthrough and my own needs, didn't plan to release this Mod on Nexus, go support [the original LevelExtender on Nexus](https://www.nexusmods.com/stardewvalley/mods/1471)_

## Install

1. [Install the latest version of SMAPI](https://smapi.io).
2. Install SpaceCore from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/1348).
3. ~~Install this mod from [NexusMods](https://www.nexusmods.com/stardewvalley/mods/1471).~~ Not released, find this code on [github](https://github.com/furudbat/Stardew-Mods/tree/master/Mods/LevelExtender)
4. Run the game using SMAPI.

## Usage

The mod will...

* **Overhaul** the leveling system. Increasing the levels the player can reach, to 100.
* Fishing is overhauled past level 10. Levels after 10 now slow down the degradation of the reeling bar.
* Optional (false by default) monster spawning everywhere. The monster spawning is based on your combat level and has a small chance of spawning at any given game tick. These monsters are generally weaker by default, but scale based on your player's Combat level. 
* Skills have the same XP system UNTIL you reach level 10. ~~The total XP to reach 100 is somewhere around 2,000,000 XP.~~ I screwup the (old) XP-Table, I'm sry =(
* Most skill changes past level 10 will show when you hover your mouse over a particular skill in the game menu.
* SDV has a host of features for each skill; based on skill level. It doesn't matter that the skills are over 10. For example: increased crop yield, based on Farming level.

## Config

~~A `username_(numbers).json` will appear in the mod's folder after you run the game once. It is **STRONGLY** recommended to not change anything in this file. It has a very high chance of causing corruption with the mod files.~~
Use the config.json or [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)

## Compatibility

* Works with Stardew Valley 1.5
* Works in single-player and Multiplayer.
* Not a 1:1 replacement for the 1.x LevelExtender
   * Old Skill API is removed
   * Different XP-Table, current Level is different ... XP should be the same
     **Please make a Backup of your saves before testing and replacing the older Mod (LevelExtender 1.x)**
     And check you Level and XP, you still can change the XP and Level via commands
* Uses a bit different XPBar from [Ui Info Suite](https://www.nexusmods.com/stardewvalley/mods/1150)
   * You need to disable the original Ui Info Suite XPBar in "Ui Info Mod Options", to avoid showing up 2 Bars and "XP gain"
      * disable "Show level up animation"
      * disable "Show expirience bar"
      * disable "Show expirience gain"

### Known Bugs

 * "Duplicating Items" new Items you didn't had in your inventory (in Multiplayer), 
   so you can basically gift Items to Player 2 and back, it triggers the "Extra Items Drops".
 * No Extra Overworld Monsters
 * Show right Level in Skill-Menu (just view your Skill-Level in the XpBar ^^")
 * SpaceCore Skills (Api) ???

## Commands

## Versions

### 2.0.0-unofficial.1-furudbat

 * Major Code refactoring
 * Change some Commands
 * Uses `config.json` and [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional) with more Options.
 * **Old Skill API is removed** Use SpaceCore for your Skills and the [Api](Api.cs) to set some values.
 * **Requires [SpaceCore](https://www.nexusmods.com/stardewvalley/mods/1348)**
 * Replace XPBar with the same Bar from [Ui Info Suite](https://www.nexusmods.com/stardewvalley/mods/1150) and add "XP gain"-Messages (also from Ui Info Suite).
    * Replace big numbers (required XP) in the XPbar with "0/10k" (instead of "0/10000")
    * Replace even bigger numbers with "XP until next level" like this "~1458"
 * Rebalance the Fishing a bit
 * Rebalance the required XP to LevelUp (Fishing and Combat needs less now XP to level up).
 * Change the Notification Messages (pop-up) (for Extra Items, expand on the already used HUB Messages).
 * add i18n support (de.json, default.json, ...) for Translations (uses ModTranslationClassBuilder).
 * **Add new Feature...**
    * Linear Crops Grow (Alternative to "Random Crops Grow"), Crops grow the same phases, depending on your (Farming) Skill-Level.
    * Add SpaceCore Skill support (need to opt-in), TODO: expand on some Skills for testing ... like Luck* or Cooking/Love for Cooking-Skill
    * Add extending Professions affects
       * "Worth more", "More Drops", "Crops Grow"
       * for example "Tiller: Crops worth 10% more." at LvL. 5 ... now at LvL. 10 it's worth 15% more, LvL. 50: 40%, LvL. 100 80% etc.
       * Same with Fishing (Angler/Fisher), Miner, Blacksmith etc.
    * Better Item Quality when harvest Crops incl. low chance for Iridium (on LvL. 100) (depending on your Skill-Level)


### 1.5.3-v001

 * Fixed item quest/item bugs
 * Added toggle for XP bars (Check mod description for commands or change config)
 * Monster spawn fixes 
 * Code optimizations

### 1.3.11

 * Initial release.


## Credits

Special thank to this awesome Developers and Modder.

 * @unidarkshin for [LevelExtender](https://github.com/unidarkshin/Stardew-Mods/tree/master/Mods/LevelExtender)
 * @spacechase0 for [SpaceCore](https://github.com/spacechase0/SpaceCore_SDV) and [GenericModConfigMenu](https://github.com/spacechase0/GenericModConfigMenu)
 * @cdaragorn for [Ui-Info-Suite](https://github.com/cdaragorn/Ui-Info-Suite)
 * @Alphablackwolf for [SkillPrestige](https://github.com/Alphablackwolf/SkillPrestige)
 * @Pathoschild for [ModTranslationClassBuilder](https://github.com/Pathoschild/SMAPI-ModTranslationClassBuilder)
