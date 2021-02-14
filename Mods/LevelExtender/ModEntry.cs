using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using LevelExtender.Framework;
using LevelExtender.Framework.Mods;
using LevelExtender.Framework.Professions;
using LevelExtender.Logging;
using LevelExtender.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LevelExtender
{
    /// <summary>The mod entry point.</summary>

    internal class ModEntry : Mod, IAssetEditor
    {
        private bool SaveIsLoaded { get; set; } = false;

        public static string ModPath { get; private set; }

        public static ModConfig Config { get; private set; }

        public static IMonitor LogMonitor { get; private set; }

        public static IModRegistry ModRegistry { get; private set; }
        public override void Entry(IModHelper helper)
        {
            // init
            LogMonitor = this.Monitor;
            ModPath = helper.DirectoryPath;
            ModRegistry = helper.ModRegistry;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            Type[] types1 = { typeof(Microsoft.Xna.Framework.Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) };
            Type[] types2 = { typeof(Item) };
            Type[] types3 = { typeof(Item), typeof(bool) };

            harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.damageMonster), types1), 
                    prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.damageMonster_Prefix))
                );
            harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool), types3),
                    prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.addItemToInventoryBool_Prefix))
                );

            RegisterGameEvents(helper.Events);

            this.Helper.Content.InvalidateCache("Data/Fish");
        }

        private void RegisterGameEvents(IModEvents events)
        {
            events.GameLoop.GameLaunched += this.onLaunched;
            events.GameLoop.OneSecondUpdateTicked += this.onOneSecondUpdateTicked;
            events.GameLoop.UpdateTicked += this.onUpdateTicked;
            events.GameLoop.SaveLoaded += this.onSaveLoaded;
            events.GameLoop.Saving += this.onSaving;
            events.GameLoop.ReturnedToTitle += this.onReturnedToTitle;
            events.GameLoop.DayStarted += this.onDayStarted;
            events.Display.Rendered += this.onDisplayRendered;
        }


        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>(); // This can also be done in your entry method if you want, but the rest needs to come in the GameLaunched event.
            var configMenuApi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            configMenuApi.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

            configMenuApi.RegisterLabel(ModManifest, "Notifications", "Notifications and Display XP Bars");
            configMenuApi.RegisterSimpleOption(ModManifest, "Show EXP Bars", "", () => Config.drawBars, (bool val) => Config.drawBars = val);
            configMenuApi.RegisterSimpleOption(ModManifest, "Show Extra Item Notifications", "", () => Config.drawExtraItemNotifications, (bool val) => Config.drawExtraItemNotifications = val);
            configMenuApi.RegisterSimpleOption(ModManifest, "Min. Item Price For Notifications", "", () => Config.minItemPriceForNotifications, (int val) => Config.minItemPriceForNotifications = val);

            configMenuApi.RegisterLabel(ModManifest, "Features", "Game changing features");
            configMenuApi.RegisterSimpleOption(ModManifest, "Enable Overworld Monsters", "Monsters spawn random on the Overworld", () => Config.OverworldMonsters, (bool val) => Config.OverworldMonsters = val);
            configMenuApi.RegisterSimpleOption(ModManifest, "Enable Crop Grow", "Crops can grow fully or faster by day (randomly, depending on your Farming-Level)", () => Config.CropsGrow, (bool val) => Config.CropsGrow = val);
            configMenuApi.RegisterSimpleOption(ModManifest, "Enable More XP from Monster", "Killing Monsters with one-hit gives more XP", () => Config.MoreEXPByOneHitKills, (bool val) => Config.MoreEXPByOneHitKills = val);
            configMenuApi.RegisterSimpleOption(ModManifest, "Enable Drop Extra Items", "Drops More Items/Harvest (randomly, depending on your Skill-Levels)", () => Config.DropExtraItems, (bool val) => Config.DropExtraItems = val);

            ModHandler.Initialise();
            levelExtender = new LevelExtender(Config, this.Helper);
            GameLocationPatch.Initialize(levelExtender);
            FarmerPatch.Initialize(levelExtender);

            // register commands
            if (Config.TestingMode)
                RegisterTestingCommands();
            RegisterCommands();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Fish");
        }

        public void Edit<T>(IAssetData asset)
        {
            /// TODO (find) util lib for data edit ...
            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach (var pair in data.ToArray())
            {
                string[] fields = pair.Value.Split('/');
                if (int.TryParse(fields[1], out int val))
                {
                    int chanceToDart = Math.Max(val - rand.Next(0, Game1.player.fishingLevel.Value / 4), val / 2);
                    fields[1] = chanceToDart.ToString();
                    data[pair.Key] = string.Join("/", fields);
                }
            }
        }


        private void onOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            if (levelExtender.Skills.Count >= 5)
            {
                levelExtender.UpdateSkillXP();
            }

            if (Config.OverworldMonsters && e.IsMultipleOf(3600))
            {
                levelExtender.CleanUpMonters();
            }
            if (Config.OverworldMonsters && !levelExtender.NoMonsters && Game1.player.currentLocation.IsOutdoors && Context.IsPlayerFree && rand.NextDouble() <= levelExtender.MonsterSpawnRate)
            {
                levelExtender.SpawnRandomMonster();
            }
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.IsMultipleOf(8))
            {
                levelExtender.UpdateBobberBar();
            }
        }


        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {

            Profession.AddMissingProfessions();

            levelExtender.InitMod();

            Helper.Content.InvalidateCache("Data/Fish");

            SaveIsLoaded = true;
        }

        private void onSaving(object sender, EventArgs e)
        {
            levelExtender.RemoveMonsters();
        }

        private void onReturnedToTitle(object sender, EventArgs e)
        {
            levelExtender.CleanUpMod();
            SaveIsLoaded = false;
        }


        private void onDayStarted(object sender, EventArgs e)
        {
            if (LocalMultiplayer.IsLocalMultiplayer())
            {
                Logger.LogWarning("Splitscreen Multiplayer is not currently supported. Mod will not load.");
                System.Environment.Exit(1);
            }

            if (Config.CropsGrow) {
                levelExtender.RandomCropGrows();
            }

            levelExtender.CleanUpLastMessage();
        }

        private void onDisplayRendered(object sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Config.drawBars)
            {
                levelExtender.DrawXPBars();
            }
        }

        private void RegisterTestingCommands()
        {
            Logger.LogInformation("Registering Testing commands...");
            SMAPICommand<ILevelExtender>.RegisterCommands(levelExtender, this.Helper.ConsoleCommands, true);
            Logger.LogInformation("Testing commands registered.");
        }

        private void RegisterCommands()
        {
            Logger.LogInformation("Registering commands...");
            SMAPICommand<ILevelExtender>.RegisterCommands(levelExtender, this.Helper.ConsoleCommands, false);
            Logger.LogInformation("Commands registered.");
        }

        private LevelExtender levelExtender;

        private static readonly Random rand = new Random(Guid.NewGuid().GetHashCode());
    }
}
