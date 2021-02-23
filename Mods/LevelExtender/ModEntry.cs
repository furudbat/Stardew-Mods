using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public ModConfig Config { get; private set; }
        public static Logger Logger { get; private set; }
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Config = helper.ReadConfig<ModConfig>();
            Logger = new Logger(Config, this.Monitor);
            levelExtender = new LevelExtender(Config, this.Helper, this.Monitor);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GameLocationPatch.Initialize(levelExtender);
            FarmerPatch.Initialize(levelExtender);
            ObjectSellToStorePricePatch.Initialize(levelExtender);
            HoeDirtPlantPatch.Initialize(levelExtender);

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
            var configMenuApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (configMenuApi != null)
            {
                configMenuApi.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));

                configMenuApi.RegisterLabel(ModManifest, "Features", "Game changing features");
                //configMenuApi.RegisterSimpleOption(ModManifest, "Crop Grow", "Crops can grow fully or faster by day. (randomly, depending on your Farming-Level)", () => Config.CropsGrow, (bool val) => Config.CropsGrow = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "More XP from Monster", "Killing Monsters with one-hit gives more XP.", () => Config.MoreEXPByOneHitKills, (bool val) => Config.MoreEXPByOneHitKills = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Drop Extra Items", "Drops more Items/Harvest. (randomly, depending on your Skill-Levels)", () => Config.DropExtraItemsByLevel, (bool val) => Config.DropExtraItemsByLevel = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Extent Profession Affects", "Drops more Items/Harvest, Crops worth more and other Profession-based increases. (based on your Profession (like Tiller) and extending 10+ Skill-Level Professions)", () => Config.DropExtraItemsByProfession, (bool val) => Config.DropExtraItemsByProfession = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Better Item Quality", "Adds better Item-Quality when Harvesting. (depending on your Skill-Level, pass Level. 13)", () => Config.BetterItemQuality, (bool val) => Config.BetterItemQuality = val);

                configMenuApi.RegisterSimpleOption(ModManifest, "Fishing Overhaul", "Extending Bobber Bar Height. (depending on your Fishing-Skill-Level)", () => Config.FishingOverhaul, (bool val) => Config.FishingOverhaul = val);

                configMenuApi.RegisterSimpleOption(ModManifest, "Extra Overworld Monsters", "Monsters spawn random on the Overworld.", () => Config.OverworldMonsters, (bool val) => Config.OverworldMonsters = val);

                configMenuApi.RegisterLabel(ModManifest, "Notifications", "Notifications and Display XP Bars");
                configMenuApi.RegisterSimpleOption(ModManifest, "Show XP Bars", "Enable XP Bar, like in Ui-Info-Suite", () => Config.DrawXPBars, (bool val) => Config.DrawXPBars = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Show XP Gain", "Show Gained XP, like in Ui-Info-Suite", () => Config.DrawXPGain, (bool val) => Config.DrawXPGain = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Extra Item Noti. Message", "Shows Message when gets Extra Items from Drops, see 'Enable Drop Extra Items'", () => Config.DrawExtraItemNotifications, (bool val) => Config.DrawExtraItemNotifications = val);
                //configMenuApi.RegisterSimpleOption(ModManifest, "Noti. in HUB", "Show Notification in HUB, otherwise in Chat.", () => Config.DrawNotificationsAsHUDMessage, (bool val) => Config.DrawNotificationsAsHUDMessage = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Min. Item Price For Noti.", "Show Extra Item Notifications with minimum Item Price.", () => Config.MinItemPriceForNotifications, (int val) => Config.MinItemPriceForNotifications = val);
                configMenuApi.RegisterSimpleOption(ModManifest, "Extra Item Noti. with Amount", "Show Extra Item Notification with Extra Item Amount, otherwise shows only a simple message.", () => Config.ExtraItemNotificationAmountMessage, (bool val) => Config.ExtraItemNotificationAmountMessage = val);
            }
            LEModHandler.Initialise(this.Monitor);

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
            /// TODO (find) util for data edit ...
            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach (var pair in data.ToArray())
            {
                string[] fields = pair.Value.Split('/');
                if (int.TryParse(fields[1], out int val))
                {
                    int chanceToDart = Math.Max(val - Game1.random.Next(0, Game1.player.fishingLevel.Value / 4), val / 2);
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
            if (Config.OverworldMonsters && !levelExtender.NoMonsters && Game1.player.currentLocation.IsOutdoors && Context.IsPlayerFree && Game1.random.NextDouble() <= levelExtender.MonsterSpawnRate)
            {
                levelExtender.SpawnRandomMonster();
            }
        }

        private void onUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Config.FishingOverhaul && e.IsMultipleOf(8))
            {
                levelExtender.UpdateBobberBar();
            }
        }


        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            levelExtender.InitMod();
            Helper.Content.InvalidateCache("Data/Fish");
        }

        private void onSaving(object sender, EventArgs e)
        {
            levelExtender.RemoveMonsters();
        }
        private void onReturnedToTitle(object sender, EventArgs e)
        {
            levelExtender.CleanUpMod();
        }
        private void onDayStarted(object sender, EventArgs e)
        {
            if (LocalMultiplayer.IsLocalMultiplayer())
            {
                Logger.LogWarning("Splitscreen Multiplayer is not currently supported. Mod will not load.");
                System.Environment.Exit(1);
            }

            if (Config.CropsGrow == CropGrowOption.RandomByLevel)
            {
                levelExtender.RandomCropGrows();
            }

            levelExtender.ResetXPBar();
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
    }
}
