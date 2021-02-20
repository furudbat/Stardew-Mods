using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using LevelExtender.Common;
using LevelExtender.Framework;
using LevelExtender.LEAPI;
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

        public ModConfig Config { get; private set; }
        public static Logger Logger { get; private set; }
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Config = helper.ReadConfig<ModConfig>();
            Logger = new Logger(Config, this.Monitor);
            levelExtender = new LevelExtender(Config, this.Helper, this.Monitor);

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

        public override object GetApi()
        {
            return levelExtender;
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
        }


        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            levelExtender.Config = Config;
            Logger.Reset(Config, this.Monitor);
            var configMenuApi = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (configMenuApi != null)
            {
                configMenuApi.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

                configMenuApi.RegisterLabel(ModManifest, "Notifications", "Notifications and Display XP Bars");
                configMenuApi.RegisterSimpleOption(ModManifest, "Show XP Bars", "Enable XP Bar", () => Config.DrawXPBars, (bool val) => Config.DrawXPBars = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Show XP Gain", "Show Gained XP", () => Config.DrawXPGain, (bool val) => Config.DrawXPGain = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Extra Item Noti. Message", "", () => Config.DrawExtraItemNotifications, (bool val) => Config.DrawExtraItemNotifications = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Noti. in HUB", "Show Notification in HUB, otherwise in Chat", () => Config.DrawNotificationsAsHUDMessage, (bool val) => Config.DrawNotificationsAsHUDMessage = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Min. Item Price For Noti.", "Show Extra Item Notifications with minimum Item Price", () => Config.MinItemPriceForNotifications, (int val) => Config.MinItemPriceForNotifications = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Extra Item Noti. with Amount", "Show Extra Item Notification with Extra Item Amount, otherwise shows only a simple message", () => Config.ExtraItemNotificationAmountMessage, (bool val) => Config.ExtraItemNotificationAmountMessage = val);

                configMenuApi.RegisterLabel(ModManifest, "Features", "Game changing features");
                configMenuApi.RegisterSimpleOption(ModManifest, "Enable Overworld Monsters", "Monsters spawn random on the Overworld", () => Config.OverworldMonsters, (bool val) => Config.OverworldMonsters = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Enable Crop Grow", "Crops can grow fully or faster by day (randomly, depending on your Farming-Level)", () => Config.CropsGrow, (bool val) => Config.CropsGrow = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Enable More XP from Monster", "Killing Monsters with one-hit gives more XP", () => Config.MoreEXPByOneHitKills, (bool val) => Config.MoreEXPByOneHitKills = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Enable Drop Extra Items", "Drops More Items/Harvest (randomly, depending on your Skill-Levels)", () => Config.DropExtraItems, (bool val) => Config.DropExtraItems = val);
            }
            LEModHandler.Initialise(this.Monitor);
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

            if (Config.CropsGrow)
            {
                levelExtender.RandomCropGrows();
            }

            levelExtender.CleanUpLastMessage();
        }

        private void RegisterTestingCommands()
        {
            Logger.LogInformation("Registering Testing commands...");
            SMAPICommand<ILevelExtender>.RegisterCommands(levelExtender, this.Config, this.Monitor, this.Helper.ConsoleCommands, true);
            Logger.LogInformation("Testing commands registered.");
        }

        private void RegisterCommands()
        {
            Logger.LogInformation("Registering commands...");
            SMAPICommand<ILevelExtender>.RegisterCommands(levelExtender, this.Config, this.Monitor, this.Helper.ConsoleCommands, false);
            Logger.LogInformation("Commands registered.");
        }

        private LevelExtender levelExtender;

        private static readonly Random rand = new Random(Guid.NewGuid().GetHashCode());
    }
}
