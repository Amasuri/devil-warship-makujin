using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DevilMakujin.Code.Graphics
{
    public class EndGameScreen
    {
        private readonly Texture2D loseGameScreen;
        private readonly string winGameTitles;
        private int fontPosY;

        private int blackoutAlpha;

        public EndGameScreen(DevimakuGame game)
        {
            loseGameScreen = game.Content.Load<Texture2D>("gui/dead");

            winGameTitles = "DEVIL WARSHIP MAKUJIN STAFF \n\n-Game Concept and Direction\nCeuteum, YoukaiDrawing, sat9\n\n- Programming Beyond Human Capabilities\nCeuteum\n\n- Graphic Design and Art Direction\nYoukaiDrawing\n\n- Music and Sound Effects\nsat9\n\n- Voice Acting\nsat9\n\n- Huge Space Whale Sounds\nCeuteum\n\n- Special Thanks\nLudum Dare Staff and Community\n\n...And you.";

            fontPosY = (int)((DevimakuGame.defGameHeight * GlobalDrawArranger.Scale) + GlobalDrawArranger.endFont.MeasureString(winGameTitles).Y);

            blackoutAlpha = 0;
        }

        public void Draw(DevimakuGame game, SpriteBatch spriteBatch, bool isGameOver)
        {
            int scale = GlobalDrawArranger.Scale;

            if (isGameOver) //loose screen
            {
                DrawTexture(spriteBatch, loseGameScreen, Vector2.Zero, scale);
            }
            else //means you've won
            {
                DrawBlackScreen(spriteBatch, game.GraphicsDevice);

                int xCenter = (int)(((DevimakuGame.defGameWidth * scale) / 2) - (GlobalDrawArranger.endFont.MeasureString(winGameTitles).X / 2));
                spriteBatch.DrawString(GlobalDrawArranger.endFont, winGameTitles, new Vector2(xCenter, fontPosY), Color.White);
                fontPosY--;
            }
        }

        private void DrawBlackScreen(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            int scale = GlobalDrawArranger.Scale;
            int winX = DevimakuGame.defGameWidth * scale;
            int winY = DevimakuGame.defGameHeight * scale;
            var rect = new Rectangle(0, 0, winX, winY);

            spriteBatch.Draw(Shader.Fill(graphics, GlobalDrawArranger.pixel, new Color(0, 0, 0, blackoutAlpha)), rect, Color.White);

            if (blackoutAlpha < 255)
                blackoutAlpha++;
        }

        public void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {

        }

        /// <summary>
        /// Wrapper method for quicker draw.
        /// </summary>
        protected void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, int scale, SpriteEffects effects = SpriteEffects.None)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, scale, effects, 0.0f);
        }
    }
}
