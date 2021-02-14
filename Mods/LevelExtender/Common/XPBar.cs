using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using LevelExtender.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace LevelExtender.Common
{
    class XPBar
    {
        public static readonly double DEFAULT_TIMER_INTERVAL = 5000;

        public LESkill Skill { get; private set; }
        public double FadeY { get; set; } = 0;
        public Timer FadeTimer { get; set; }
        public DateTime FadeTime { get; set; }

        public XPBar(ILevelExtender levelExtender, LESkill skill)
        {
            Skill = skill;
            FadeTimer = new Timer(DEFAULT_TIMER_INTERVAL);
            FadeTimer.Elapsed += delegate { levelExtender.EndXPBar(this.Skill.Skill.Type); };
            FadeTimer.AutoReset = false;
            FadeTimer.Enabled = true;
            FadeTime = DateTime.Now;
        }

        public void resetY() {
            FadeY = 0;
        }

        public void DrawXPBar(DateTime lastDrawXPBarsTime, int? topYFade = null, int index = 0)
        {
            float barScale = 1.0f; ///< TODO get Zoom UI scale

            string skillName = Skill.Skill.Type.Name;
            string displaySkillName = string.Join(" ", skillName.ToCharArray()).Trim();

            int startX = 8;
            int startY = 8;
            int sep = (int)(30 * barScale);
            int barSep = (int)(60 * barScale);

            int level = Skill.Level;
            int startXP = (level > 0)? Skill.getRequiredXP(level - 1) : 0;
            int curXP = Skill.XP;
            int changedXP = Skill.ChangedXP;
            int endXP = Skill.getRequiredXP(level);
            int maxXP = endXP - startXP;
            int diffXP = curXP - startXP;
            double deltaTime = DateTime.Now.Subtract(FadeTime).TotalMilliseconds;

            float opacity = 1.0f;
            if (deltaTime >= 0 && deltaTime <= 1000)
            {
                opacity = ((float)deltaTime) / 1200.0f;
            }
            else if (deltaTime > 1000 && deltaTime <= 4000)
            {
                opacity = 0.833f;
            }
            else
            {
                opacity = ((float)(5000 - deltaTime)) / 1200.0f;
            }

            int uiBarWidth = (int)(198 * barScale);
            double barWidth = uiBarWidth / (double) maxXP;
            int curXPBarWidth = (int)Math.Round(diffXP * barWidth) + 1;
            int changedXPBarWidth = (int)Math.Round(changedXP * barWidth);

            if (topYFade != null) {
                FadeY = topYFade ?? 0;
            } 
            else
            {
                if (FadeY < 0)
                {
                    double ms = (DateTime.Now - lastDrawXPBarsTime).TotalMilliseconds;
                    double addv = (FadeY + (ms / 15.625 * barScale));
                    FadeY = (addv >= 0 ? 0 : addv);
                }
                else if (deltaTime >= 4000)
                {
                    double addv = (deltaTime - 4000) / 15.625 * barScale;
                    FadeY = (addv >= 64 ? 64 : addv);
                }
            }

            Vector2 r1d = new Vector2((float)Math.Round(214 * barScale), (float)Math.Round(64 * barScale));
            Vector2 r2d = new Vector2((float)Math.Round(210 * barScale), (float)Math.Round(60 * barScale));
            Vector2 r3d = new Vector2((float)Math.Round(200 * barScale), (float)Math.Round(20 * barScale));
            Vector2 r4d = new Vector2(curXPBarWidth, (float)Math.Round(18 * barScale));
            Vector2 r5d = new Vector2(changedXPBarWidth, (float)Math.Round(18 * barScale));

            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX - 7, startY + (barSep * index) - 7 - (int) FadeY, (int)r1d.X, (int)r1d.Y), Color.DarkRed * opacity);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX - 5, startY + (barSep * index) - 5 - (int) FadeY, (int)r2d.X, (int)r2d.Y), new Color(210, 173, 85) * opacity);

            Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{displaySkillName}", new Vector2((int)Math.Round(((startX - 7 + r1d.X) / 2.0) - (Game1.dialogueFont.MeasureString(displaySkillName).X * (Game1.pixelZoom / 6.0f / 2.0f) * barScale)), (startY - 3 + (barSep * index) - (int) FadeY) * barScale), new Color(30, 3, 0) * (opacity * 1.1f), 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6f * barScale), SpriteEffects.None, 0.5f);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{displaySkillName}", new Vector2((int)Math.Round(((startX - 7 + r1d.X) / 2.0) - (Game1.dialogueFont.MeasureString(displaySkillName).X * (Game1.pixelZoom / 6.0f / 2.0f) * barScale)) + 1, (startY - 3 + (barSep * index) - ((int) FadeY) + 1) * barScale), new Color(90, 35, 0) * (opacity), 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6.0f * barScale), SpriteEffects.None, 0.5f);

            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX, startY + (barSep * index) + sep - (int) FadeY, (int)r3d.X, (int)r3d.Y), Color.Black * opacity);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX + 1, startY + (barSep * index) + sep + 1 - (int) FadeY, curXPBarWidth, (int)r4d.Y), Color.SeaGreen * opacity);
            Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX + 1 + curXPBarWidth - changedXPBarWidth, startY + (barSep * index) + sep + 1 - (int) FadeY, changedXPBarWidth, (int)r5d.Y), Color.Turquoise * opacity);

            Vector2 mPos = new Vector2(Game1.getMouseX(), Game1.getMouseY());
            Vector2 bCenter = new Vector2(startX + (200 / 2), startY + (barSep * index) + sep + (20 / 2) - (int) FadeY);
            float dist = Vector2.Distance(mPos, bCenter);

            if (dist <= 250f)
            {
                float fade = Math.Min(25f / dist, 1.0f);
                string bar_text = $"{curXP} / {endXP}";
                Game1.spriteBatch.DrawString(Game1.dialogueFont, bar_text, new Vector2((int)Math.Round(((startX + 200) / 2.0) - (Game1.dialogueFont.MeasureString(bar_text).X * (Game1.pixelZoom / 10.0f / 2.0f))), startY + (barSep * index) + sep + 1 - (int) FadeY), Color.White * fade * (opacity + 0.05f), 0.0f, Vector2.Zero, (Game1.pixelZoom / 10f), SpriteEffects.None, 0.5f);

                //Logger.LogDebug($"DrawXPBar: draw uiBarWidth {uiBarWidth}");
                //Logger.LogDebug($"DrawXPBar: draw barWidth {barWidth}");
                //Logger.LogDebug($"DrawXPBar: draw changedXPBarWidth {changedXPBarWidth}");
            }
        }

    }
}
