using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LevelExtender.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace LevelExtender.UIElements
{
    /// <summary>
    /// ExperienceBar from Ui-Info-Suite by Cdaragorn - https://github.com/cdaragorn/Ui-Info-Suite
    /// </summary>
    class ExtendedExperienceBar : IDisposable
    {
        private const int MaxBarWidth = 180;

        private readonly Dictionary<string, int> _lastXPs = new Dictionary<string, int>();
        private readonly List<ExperiencePointDisplay> _XPDisplays = new List<ExperiencePointDisplay>();
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
        private int _currentXP = 0;
        private int _requiredXPPrevLevel = -1;
        private int _requiredXPNextLevel = -1;
        private int _maxSkillLevel = -1;

        private readonly ILevelExtender _levelExtender;

        public ExtendedExperienceBar(IModHelper helper, ILevelExtender levelExtender)
        {
            _helper = helper;
            _timeToDisappear.Elapsed += StopTimerAndFadeBarOut;
            _helper.Events.Display.RenderingHud += OnDisplayRenderingHud;
            _helper.Events.Player.Warped += PlayerOnWarped;
            _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            _levelExtender = levelExtender;
            foreach (var skill in _levelExtender.VanillaSkills)
            {
                _lastXPs.Add(skill.Name, skill.XP);
            }
        }

        public void Dispose()
        {
            _levelExtender.OnXPChanged -= OnXPChanged;
            _helper.Events.Player.LevelChanged -= OnPlayerLevelChanged;
            _helper.Events.Display.RenderingHud -= OnDisplayRenderingHud;
            _helper.Events.Player.Warped -= PlayerOnWarped;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            if (_timeToDisappear != null)
            {
                _timeToDisappear.Elapsed -= StopTimerAndFadeBarOut;
                _timeToDisappear.Stop();
                _timeToDisappear.Dispose();
                _timeToDisappear = null;
            }
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
            if (!Context.IsWorldReady)
                return;

            if (e.IsLocalPlayer)
                _XPDisplays.Clear();
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
            if (currentItem != null && _previousItem != currentItem)
            {
                string skillName;
                if (currentItem is FishingRod)
                {
                    skillName = SkillsList.DefaultSkillNames.Fishing;
                }
                else if (currentItem is Pickaxe)
                {
                    skillName = SkillsList.DefaultSkillNames.Mining;
                }
                else if (currentItem is MeleeWeapon && currentItem.Name != "Scythe" && currentItem.Name != "Golden Scythe")
                {
                    skillName = SkillsList.DefaultSkillNames.Combat;
                }
                else if (currentItem is MeleeWeapon && (currentItem.Name == "Scythe" || currentItem.Name == "Golden Scythe"))
                {
                    skillName = SkillsList.DefaultSkillNames.Farming;
                }
                else if (currentItem.Name == "Watering Can" || currentItem.Name == "Cooper Watering Can" || currentItem.Name == "Steel Watering Can" || currentItem.Name == "Gold Watering Can" || currentItem.Name == "Iridium Watering Can")
                {
                    skillName = SkillsList.DefaultSkillNames.Farming;
                }
                else if (Game1.currentLocation is Farm && !(currentItem is Axe))
                {
                    skillName = SkillsList.DefaultSkillNames.Farming;
                }
                else
                {
                    skillName = SkillsList.DefaultSkillNames.Foraging;
                }
                int xp = _levelExtender.GetSkillCurrentXP(skillName);
                int skillLevel = _levelExtender.GetSkillLevel(skillName);
                int requiredXPNextLevel = _levelExtender.GetXPRequiredToLevel(skillName, skillLevel);
                int requiredXPPrevLevel = (skillLevel > 0) ? _levelExtender.GetXPRequiredToLevel(skillName, skillLevel - 1) : 0;
                int maxSkillLevel = _levelExtender.GetSkillMaxLevel(skillName);

                ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnUpdateTicked change item: skill: {skillName}, lvl: {skillLevel}, xp: {xp}, prev. {requiredXPPrevLevel}/{requiredXPNextLevel}");

                if (_showExperienceBar && skillName != "")
                {
                    ShowExperienceBar(skillName, xp, skillLevel, requiredXPNextLevel, requiredXPPrevLevel, maxSkillLevel);
                }
                _previousItem = currentItem;
            }
        }
        private void OnXPChanged(object sender, LEXPEventArgs args)
        {
            string skillName = args.SkillName;
            int xp = args.NewXP;
            int skillLevel = args.CurrentLevel;
            int changedXP = args.ChangedXP;
            int requiredXPNextLevel = _levelExtender.GetXPRequiredToLevel(skillName, skillLevel);
            int requiredXPPrevLevel = (skillLevel > 0) ? _levelExtender.GetXPRequiredToLevel(skillName, skillLevel - 1) : 0;
            int maxSkillLevel = _levelExtender.GetSkillMaxLevel(skillName);

            ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnXPChanged: {skillName}, lvl: {skillLevel}, xp: {xp} ({changedXP}), prev. {requiredXPPrevLevel}/{requiredXPNextLevel}");

            if (_showExperienceBar && changedXP > 0 && skillName != "")
            {
                ShowExperienceBar(skillName, xp, skillLevel, requiredXPNextLevel, requiredXPPrevLevel, maxSkillLevel);
            }
            if (_showExperienceGain &&
                changedXP > 0 &&
                _lastXPs.ContainsKey(skillName))
            {
                _XPDisplays.Add(
                    new ExperiencePointDisplay(
                        changedXP,
                        Game1.player.getLocalPosition(Game1.viewport)));
            }

            if (_lastXPs.ContainsKey(skillName))
            {
                _lastXPs[skillName] = _currentXP;
            }
            else
            {
                _lastXPs.Add(skillName, _currentXP);
            }
        }

        /// <summary>Raised after a player skill level changes. This happens as soon as they level up, not when the game notifies the player after their character goes to bed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPlayerLevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            ModEntry.Logger.LogDebug($"ExtendedExperienceBar.OnLevelChanged: {e.Skill} {e.OldLevel} -> {e.NewLevel}");

            if (_showLevelUpAnimation && e.IsLocalPlayer)
            {
                string skillName = "";
                switch (e.Skill)
                {
                    case StardewModdingAPI.Enums.SkillType.Combat: skillName = SkillsList.DefaultSkillNames.Combat; break;
                    case StardewModdingAPI.Enums.SkillType.Farming: skillName = SkillsList.DefaultSkillNames.Farming; break;
                    case StardewModdingAPI.Enums.SkillType.Fishing: skillName = SkillsList.DefaultSkillNames.Fishing; break;
                    case StardewModdingAPI.Enums.SkillType.Foraging: skillName = SkillsList.DefaultSkillNames.Foraging; break;
                    case StardewModdingAPI.Enums.SkillType.Mining: skillName = SkillsList.DefaultSkillNames.Mining; break;
                }
                int skillLevel = e.NewLevel;
                int xp = _levelExtender.GetSkillCurrentXP(skillName);
                int requiredXPNextLevel = _levelExtender.GetXPRequiredToLevel(skillName, skillLevel);
                int requiredXPPrevLevel = (skillLevel > 0) ? _levelExtender.GetXPRequiredToLevel(skillName, skillLevel - 1) : 0;
                int maxSkillLevel = _levelExtender.GetSkillMaxLevel(skillName);

                if (_showExperienceBar)
                {
                    _shouldDrawLevelUp = true;
                    ShowExperienceBar(skillName, xp, skillLevel, requiredXPNextLevel, requiredXPPrevLevel, maxSkillLevel);

                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(_levelUpPauseTime);
                        _shouldDrawLevelUp = false;
                    });
                }
            }
        }

        /// <summary>Raised before drawing the HUD (item toolbar, clock, etc) to the screen. The vanilla HUD may be hidden at this point (e.g. because a menu is open).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDisplayRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

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
                        _helper.Translation.Get(I18n.LevelUp()),
                        Color.DarkSlateGray,
                        Color.PaleTurquoise,
                        new Vector2(
                            playerLocalPosition.X - 28,
                            playerLocalPosition.Y - 130));
                }

                if (_showExperienceGain)
                {
                    for (int i = _XPDisplays.Count - 1; i >= 0; --i)
                    {
                        if (_XPDisplays[i].IsInvisible)
                        {
                            _XPDisplays.RemoveAt(i);
                        }
                        else
                        {
                            _XPDisplays[i].Draw();
                        }
                    }
                }

                if (_experienceBarShouldBeVisible &&
                    _showExperienceBar)
                {
                    int maxExperience = (_requiredXPNextLevel > 0) ? _requiredXPNextLevel - _requiredXPPrevLevel : -1;
                    int currentExperience = _currentXP - _requiredXPPrevLevel;
                    int progressBarWidth = (maxExperience > 0) ? (int)((double)currentExperience / maxExperience * MaxBarWidth) : 0;

                    ModEntry.Logger.LogDebug($"DrawExperienceBar: max exp. {maxExperience}, xp: {currentExperience}, bar width: {progressBarWidth}");

                    DrawExperienceBar(progressBarWidth, _currentSkillLevel, currentExperience, maxExperience, _maxSkillLevel);
                }
            }
        }

        private void ShowExperienceBar(string currentLevelSkillName, int currentXP, int currentSkillLevel, int requiredXPNextLevel, int requiredXPPrevLevel, int maxSkillLevel)
        {
            _currentXP = currentXP;
            _currentSkillLevel = currentSkillLevel;
            _requiredXPPrevLevel = requiredXPPrevLevel;
            _requiredXPNextLevel = requiredXPNextLevel;
            _maxSkillLevel = maxSkillLevel;

            switch (currentLevelSkillName)
            {
                case SkillsList.DefaultSkillNames.Farming:
                    {
                        _experienceFillColor = new Color(255, 251, 35, 0.38f);
                        _experienceIconPosition.X = 10;
                        break;
                    }

                case SkillsList.DefaultSkillNames.Fishing:
                    {
                        _experienceFillColor = new Color(17, 84, 252, 0.63f);
                        _experienceIconPosition.X = 20;
                        break;
                    }

                case SkillsList.DefaultSkillNames.Foraging:
                    {
                        _experienceFillColor = new Color(0, 234, 0, 0.63f);
                        _experienceIconPosition.X = 60;
                        break;
                    }

                case SkillsList.DefaultSkillNames.Mining:
                    {
                        _experienceFillColor = new Color(145, 104, 63, 0.63f);
                        _experienceIconPosition.X = 30;
                        break;
                    }

                case SkillsList.DefaultSkillNames.Combat:
                    {
                        _experienceFillColor = new Color(204, 0, 3, 0.63f);
                        _experienceIconPosition.X = 120;
                        break;
                    }
            }
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

        private void DrawExperienceBar(int progressBarWidth, int currentLevel, int currentExperience, int maxExperience, int maxSkillLevel)
        {
            float leftSide = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left;

            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                leftSide += (Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - (Game1.currentLocation.map.Layers[0].LayerWidth * Game1.tileSize)) / 2;
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
                    progressBarWidth,
                    31),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63,
                    Math.Min(4, progressBarWidth),
                    31),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 63,
                    progressBarWidth,
                    4),
                _experienceFillColor);

            Game1.spriteBatch.Draw(
                Game1.staminaRect,
                new Rectangle(
                    (int)leftSide + 32,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 36,
                    progressBarWidth,
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

            if (currentLevel < maxSkillLevel && textureComponent.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                string strCurrentExperience = currentExperience.ToString();
                string strMaxExperience = maxExperience.ToString();
                strCurrentExperience = (currentExperience >= 10000) ? strCurrentExperience.Substring(0, strCurrentExperience.Length - 3) : strCurrentExperience;
                strMaxExperience = (maxExperience >= 10000) ? maxExperience.ToString().Substring(0, strMaxExperience.Length - 3) : strMaxExperience;

                string txtCurrentExperience = (currentExperience >= 10000) ? I18n.KAmountWithUnit(amount: strCurrentExperience) : strCurrentExperience;
                string txtMaxExperience = (maxExperience >= 10000) ? I18n.KAmountWithUnit(amount: strMaxExperience) : strMaxExperience;
                if (maxExperience < 0)
                {
                    txtMaxExperience = "---";
                }

                string barText = (maxExperience >= 0) ? $"{txtCurrentExperience}/{txtMaxExperience}" : txtCurrentExperience;
                if (currentExperience >= 10000 && maxExperience >= 10000)
                {
                    barText = I18n.XPUntilNextLevel(amount: maxExperience - currentExperience);
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
                        leftSide + ((currentLevel >= 100) ? 96 : (currentLevel >= 10) ? 76 : 54),
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
