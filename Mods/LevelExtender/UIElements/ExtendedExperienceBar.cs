using LevelExtender.LEAPI;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace LevelExtender.UIElements
{
    // ExperienceBar from Ui-Info-Suite by Cdaragorn - https://github.com/cdaragorn/Ui-Info-Suite
    class ExtendedExperienceBar : IDisposable
    {
        private const int MaxBarWidth = 175;

        private Dictionary<string, int> _currentExperience = new Dictionary<string, int>();
        private readonly List<ExperiencePointDisplay> _experiencePointDisplays = new List<ExperiencePointDisplay>();
        private readonly TimeSpan _levelUpPauseTime = TimeSpan.FromSeconds(2);
        private readonly Color _iconColor = Color.White;
        private Color _experienceFillColor = Color.Blue;
        private Rectangle _experienceIconPosition = new Rectangle(10, 428, 10, 10);
        private Item _previousItem = null;
        private bool _experienceBarShouldBeVisible = false;
        private bool _shouldDrawLevelUp = false;
        private System.Timers.Timer _timeToDisappear = new System.Timers.Timer();
        private readonly TimeSpan _timeBeforeExperienceBarFades = TimeSpan.FromSeconds(8);
        private Rectangle _levelUpIconRectangle = new Rectangle(120, 428, 10, 10);
        private bool _allowExperienceBarToFadeOut = true;
        private bool _showLevelUpAnimation = true;
        private bool _showExperienceBar = true;
        private bool _showExperienceGain = true;
        private readonly IModHelper _helper;

        private int _currentSkillLevel = 0;
        private int _experienceRequiredToLevel = -1;
        private int _experienceFromPreviousLevels = -1;
        private int _experienceEarnedThisLevel = -1;

        private LEModApi _levelExtender;

        public ExtendedExperienceBar(IModHelper helper, LEModApi levelExtender)
        {
            _helper = helper;
            _timeToDisappear.Elapsed += StopTimerAndFadeBarOut;
            _helper.Events.Display.RenderingHud += OnDisplayRenderingHud;
            _helper.Events.Player.Warped += PlayerOnWarped;
            _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            _levelExtender = levelExtender;
            foreach (var skill in _levelExtender.GetSkills())
            {
                _currentExperience.Add(skill.Name, skill.XP);
            }
        }

        public void Dispose()
        {
            _levelExtender.OnXPChanged -= OnXPChanged;
            _helper.Events.Player.LevelChanged -= OnPlayerLevelChanged;
            _helper.Events.Display.RenderingHud -= OnDisplayRenderingHud;
            _helper.Events.Player.Warped -= PlayerOnWarped;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            _timeToDisappear.Elapsed -= StopTimerAndFadeBarOut;
            _timeToDisappear.Stop();
            _timeToDisappear.Dispose();
            _timeToDisappear = null;
        }

        public void ToggleLevelUpAnimation(bool showLevelUpAnimation)
        {
            _showLevelUpAnimation = showLevelUpAnimation;
            _helper.Events.Player.LevelChanged -= OnPlayerLevelChanged;

            if (_showLevelUpAnimation)
            {
                _helper.Events.Player.LevelChanged += OnPlayerLevelChanged;
            }
        }

        public void ToggleExperienceBarFade(bool allowExperienceBarToFadeOut)
        {
            _allowExperienceBarToFadeOut = allowExperienceBarToFadeOut;
        }

        public void ToggleShowExperienceBar(bool showExperienceBar)
        {
            _showExperienceBar = showExperienceBar;
            if (_showExperienceBar || _showExperienceGain)
            {
                _levelExtender.OnXPChanged += OnXPChanged;
            }
            else
            {
                _levelExtender.OnXPChanged -= OnXPChanged;
            }
        }
        public void ToggleShowExperienceGain(bool showExperienceGain)
        {
            _showExperienceGain = showExperienceGain;
            if (_showExperienceGain || _showExperienceBar)
            {
                _levelExtender.OnXPChanged += OnXPChanged;
            }
            else
            {
                _levelExtender.OnXPChanged -= OnXPChanged;
            }
        }

        /// <summary>Raised after a player skill level changes. This happens as soon as they level up, not when the game notifies the player after their character goes to bed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayerLevelChanged(object sender, LevelChangedEventArgs e)
        {
            ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnLevelChanged: {e.Skill} {e.OldLevel} -> {e.NewLevel}");

            if (_showLevelUpAnimation && e.IsLocalPlayer)
            {
                switch (e.Skill)
                {
                    case StardewModdingAPI.Enums.SkillType.Combat: _levelUpIconRectangle.X = 120; break;
                    case StardewModdingAPI.Enums.SkillType.Farming: _levelUpIconRectangle.X = 10; break;
                    case StardewModdingAPI.Enums.SkillType.Fishing: _levelUpIconRectangle.X = 20; break;
                    case StardewModdingAPI.Enums.SkillType.Foraging: _levelUpIconRectangle.X = 60; break;
                    case StardewModdingAPI.Enums.SkillType.Mining: _levelUpIconRectangle.X = 30; break;
                }

                if (_showExperienceBar)
                {
                    _shouldDrawLevelUp = true;
                    ShowExperienceBar();

                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(_levelUpPauseTime);
                        _shouldDrawLevelUp = false;
                    });
                }
            }
        }

        private void StopTimerAndFadeBarOut(object sender, ElapsedEventArgs e)
        {
            _timeToDisappear?.Stop();
            _experienceBarShouldBeVisible = false;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerOnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                _experiencePointDisplays.Clear();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!e.IsMultipleOf(15)) // quarter second
                return;

            Item currentItem = Game1.player.CurrentItem;
            string currentLevelSkillName = "";

            if (_previousItem != currentItem)
            {
                if (currentItem is FishingRod)
                {
                    _experienceFillColor = new Color(17, 84, 252, 0.63f);
                    currentLevelSkillName = DefaultSkillNames.Fishing;
                    _experienceIconPosition.X = 20;
                    _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);
                }
                else if (currentItem is Pickaxe)
                {
                    _experienceFillColor = new Color(145, 104, 63, 0.63f);
                    currentLevelSkillName = DefaultSkillNames.Mining;
                    _experienceIconPosition.X = 30;
                    _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);
                }
                else if (currentItem is MeleeWeapon && currentItem.Name != "Scythe")
                {
                    _experienceFillColor = new Color(204, 0, 3, 0.63f);
                    currentLevelSkillName = DefaultSkillNames.Combat;
                    _experienceIconPosition.X = 120;
                    _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);
                }
                else if (Game1.currentLocation is Farm && !(currentItem is Axe))
                {
                    _experienceFillColor = new Color(255, 251, 35, 0.38f);
                    currentLevelSkillName = DefaultSkillNames.Farming;
                    _experienceIconPosition.X = 10;
                    _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);
                }
                else
                {
                    _experienceFillColor = new Color(0, 234, 0, 0.63f);
                    currentLevelSkillName = DefaultSkillNames.Foraging;
                    _experienceIconPosition.X = 60;
                    _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);
                }

                if (currentLevelSkillName != "" && _experienceRequiredToLevel <= 0 && _currentExperience.ContainsKey(currentLevelSkillName))
                {
                    _experienceEarnedThisLevel = _levelExtender.GetSkillCurrentXP(currentLevelSkillName);
                    _experienceFromPreviousLevels = _currentExperience[currentLevelSkillName] - _experienceEarnedThisLevel;
                    _experienceRequiredToLevel = _levelExtender.GetSkillRequiredXP(currentLevelSkillName) + _experienceFromPreviousLevels;
                }

                if (_showExperienceBar && currentLevelSkillName != "")
                {
                    ShowExperienceBar();
                }
                _previousItem = currentItem;
            }
        }
        private void OnXPChanged(object sender, LEXPEventArgs args)
        {
            string currentLevelSkillName = args.SkillName;
            int currentXP = _levelExtender.GetSkillCurrentXP(currentLevelSkillName);
            _currentSkillLevel = _levelExtender.GetSkillLevel(currentLevelSkillName);

            ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnXPChanged: {currentLevelSkillName}, lvl: {_currentSkillLevel}, xp: {currentXP}");

            if (currentLevelSkillName != null)
            {
                switch (currentLevelSkillName)
                {
                    case DefaultSkillNames.Farming:
                        {
                            _experienceFillColor = new Color(255, 251, 35, 0.38f);
                            _experienceIconPosition.X = 10;
                            break;
                        }

                    case DefaultSkillNames.Fishing:
                        {
                            _experienceFillColor = new Color(17, 84, 252, 0.63f);
                            _experienceIconPosition.X = 20;
                            break;
                        }

                    case DefaultSkillNames.Foraging:
                        {
                            _experienceFillColor = new Color(0, 234, 0, 0.63f);
                            _experienceIconPosition.X = 60;
                            break;
                        }

                    case DefaultSkillNames.Mining:
                        {
                            _experienceFillColor = new Color(145, 104, 63, 0.63f);
                            _experienceIconPosition.X = 30;
                            break;
                        }

                    case DefaultSkillNames.Combat:
                        {
                            _experienceFillColor = new Color(204, 0, 3, 0.63f);
                            _experienceIconPosition.X = 120;
                            break;
                        }
                }

                _experienceRequiredToLevel = (_currentSkillLevel >= 0) ? GetExperienceRequiredToLevel(currentLevelSkillName, _currentSkillLevel) : 0;
                _experienceFromPreviousLevels = (_currentSkillLevel >= 1) ? GetExperienceRequiredToLevel(currentLevelSkillName, _currentSkillLevel - 1) : 0;
                _experienceEarnedThisLevel = currentXP - _experienceFromPreviousLevels;
                //int experiencePreviouslyEarnedThisLevel = _currentExperience[currentLevelSkillName] - _experienceFromPreviousLevels;

                if (_experienceRequiredToLevel <= 0 && _currentExperience.ContainsKey(currentLevelSkillName))
                {
                    _experienceEarnedThisLevel = _levelExtender.GetSkillCurrentXP(currentLevelSkillName);
                    _experienceFromPreviousLevels = _currentExperience[currentLevelSkillName] - _experienceEarnedThisLevel;
                    _experienceRequiredToLevel = _levelExtender.GetSkillRequiredXP(currentLevelSkillName) + _experienceFromPreviousLevels;
                }

                if (_showExperienceBar)
                {
                    ShowExperienceBar();
                }
                if (_showExperienceGain &&
                    _experienceRequiredToLevel > 0 &&
                    _currentExperience.ContainsKey(currentLevelSkillName))
                {
                    int currentExperienceToUse = _levelExtender.GetSkillCurrentXP(currentLevelSkillName);
                    int previousExperienceToUse = _currentExperience[currentLevelSkillName];
                    int experienceGain = currentExperienceToUse - previousExperienceToUse;

                    if (experienceGain > 0)
                    {
                        _experiencePointDisplays.Add(
                            new ExperiencePointDisplay(
                                experienceGain,
                                Game1.player.getLocalPosition(Game1.viewport)));
                    }
                }

                if (_currentExperience.ContainsKey(currentLevelSkillName))
                    _currentExperience[currentLevelSkillName] = _levelExtender.GetSkillCurrentXP(currentLevelSkillName);
                else
                    _currentExperience.Add(currentLevelSkillName, _levelExtender.GetSkillCurrentXP(currentLevelSkillName));
            }

            ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnXPChanged: prev. {_experienceFromPreviousLevels}, earned {_experienceEarnedThisLevel}, req. {_experienceRequiredToLevel}");
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDisplayRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Game1.eventUp)
            {
                if (_shouldDrawLevelUp)
                {
                    Vector2 playerLocalPosition = Game1.player.getLocalPosition(Game1.viewport);
                    Game1.spriteBatch.Draw(
                        Game1.mouseCursors,
                        new Vector2(
                            playerLocalPosition.X - 74,
                            playerLocalPosition.Y - 130),
                        _levelUpIconRectangle,
                        _iconColor,
                        0,
                        Vector2.Zero,
                        Game1.pixelZoom,
                        SpriteEffects.None,
                        0.85f);

                    Game1.drawWithBorder(
                        _helper.Translation.Get(LanguageKeys.LevelUp),
                        Color.DarkSlateGray,
                        Color.PaleTurquoise,
                        new Vector2(
                            playerLocalPosition.X - 28,
                            playerLocalPosition.Y - 130));
                }

                if (_showExperienceGain)
                {
                    for (int i = _experiencePointDisplays.Count - 1; i >= 0; --i)
                    {
                        if (_experiencePointDisplays[i].IsInvisible)
                        {
                            _experiencePointDisplays.RemoveAt(i);
                        }
                        else
                        {
                            _experiencePointDisplays[i].Draw();
                        }
                    }
                }

                if (_experienceRequiredToLevel > 0 &&
                    _experienceBarShouldBeVisible &&
                    _showExperienceBar)
                {
                    int experienceDifferenceBetweenLevels = _experienceRequiredToLevel - _experienceFromPreviousLevels;
                    int barWidth = (experienceDifferenceBetweenLevels > 0) ? (int)((double)_experienceEarnedThisLevel / experienceDifferenceBetweenLevels * MaxBarWidth) : 0;

                    DrawExperienceBar(barWidth, _experienceEarnedThisLevel, experienceDifferenceBetweenLevels, _currentSkillLevel);
                }

            }
        }

        private int GetExperienceRequiredToLevel(string skillName, int level)
        {
            return _levelExtender.GetXPRequiredToLevel(skillName, level);
        }

        private void ShowExperienceBar()
        {
            if (_timeToDisappear != null)
            {
                if (_allowExperienceBarToFadeOut)
                {
                    _timeToDisappear.Interval = _timeBeforeExperienceBarFades.TotalMilliseconds;
                    _timeToDisappear.Start();
                }
                else
                {
                    _timeToDisappear.Stop();
                }
            }

            _experienceBarShouldBeVisible = true;
        }

        private void DrawExperienceBar(int barWidth, int experienceGainedThisLevel, int experienceRequiredForNextLevel, int currentLevel)
        {
            float leftSide = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left;

            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                int num3 = Game1.currentLocation.map.Layers[0].LayerWidth * Game1.tileSize;
                leftSide += (Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - num3) / 2;
            }

            Game1.drawDialogueBox(
                (int)leftSide,
                Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 160,
                240,
                160,
                false,
                true);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63,
                    barWidth,
                    31),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63,
                    Math.Min(4, barWidth),
                    31),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63,
                    barWidth,
                    4),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 36,
                    barWidth,
                    4),
                _experienceFillColor);

            ClickableTextureComponent textureComponent =
                new ClickableTextureComponent(
                    "",
                    new Rectangle(
                        (int)leftSide - 36,
                        Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 80,
                        260,
                        100),
                    "",
                    "",
                    Game1.mouseCursors,
                    new Rectangle(0, 0, 0, 0),
                    Game1.pixelZoom);

            if (textureComponent.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                string strExperienceGainedThisLevelAmount = (experienceGainedThisLevel >= 10000) ? experienceGainedThisLevel.ToString().Substring(0, 2) : experienceGainedThisLevel.ToString();
                string strExperienceRequiredForNextLevelAmount = (experienceRequiredForNextLevel >= 10000) ? experienceRequiredForNextLevel.ToString().Substring(0, 2) : experienceRequiredForNextLevel.ToString();

                string strExperienceGainedThisLevel = (experienceGainedThisLevel >= 10000) ? this._helper.Translation.Get(LanguageKeys.KAmountWithUnit, new { amount = strExperienceGainedThisLevelAmount }).Default(strExperienceGainedThisLevelAmount + "k") : strExperienceGainedThisLevelAmount;
                string strExperienceRequiredForNextLevel = (experienceRequiredForNextLevel >= 10000) ? this._helper.Translation.Get(LanguageKeys.KAmountWithUnit, new { amount = strExperienceRequiredForNextLevelAmount }).Default(strExperienceRequiredForNextLevelAmount + "k") : strExperienceRequiredForNextLevelAmount;

                string barText = $"{strExperienceGainedThisLevel}/{strExperienceRequiredForNextLevel}";
                if (experienceGainedThisLevel >= 10000 && experienceRequiredForNextLevel >= 10000) {
                    barText = this._helper.Translation.Get(LanguageKeys.XPUntilNextLevel, new { amount = experienceRequiredForNextLevel - experienceGainedThisLevel });
                }

                Game1.drawWithBorder(
                    barText,
                    Color.Black,
                    Color.Black,
                    new Vector2(
                        leftSide + 33,
                        Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 70));
            }
            else
            {
                Game1.spriteBatch.Draw(
                    Game1.mouseCursors,
                    new Vector2(
                        leftSide + ((currentLevel >= 10) ? 84 : 54),
                        Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 62),
                    _experienceIconPosition,
                    _iconColor,
                    0,
                    Vector2.Zero,
                    2.9f,
                    SpriteEffects.None,
                    0.85f);

                Game1.drawWithBorder(
                    currentLevel.ToString(),
                    Color.Black * 0.6f,
                    Color.Black,
                    new Vector2(
                        leftSide + 33,
                        Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 70));
            }
        }

    }
}
