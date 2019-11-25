using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DevilMakujin.Code.Graphics
{
    public class ParallaxSprite
    {
        private readonly Texture2D canvas;
        private readonly List<Texture2D> parallax;
        private readonly int[] parallaxOrderId;

        public ParallaxSprite(DevimakuGame game)
        {
            this.canvas = game.Content.Load<Texture2D>("gui/parallax3");
            this.parallax = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("parallax/tile1"),
                game.Content.Load<Texture2D>("parallax/tile2"),
                game.Content.Load<Texture2D>("parallax/tile3"),
                game.Content.Load<Texture2D>("parallax/tile4"),
            };

            this.parallaxOrderId = new int[]
            {
                0, 2, 1,
                3, 0, 2,
                2, 1, 3
            };
        }

        public void Render(SpriteBatch spriteBatch, int scale, Vector2 plAbsPos, Rectangle mapBounds)
        {
            Vector2 plPos = -plAbsPos;
            int winX = DevimakuGame.defGameWidth * GlobalDrawArranger.Scale; //Assuming that canvas = screen size // parallax[0].Width * GlobalDrawArranger.Scale; //
            int winY = DevimakuGame.defGameHeight * GlobalDrawArranger.Scale; //parallax[0].Height * GlobalDrawArranger.Scale; //

            List<Vector2> renderPos = new List<Vector2>
            {
                new Vector2( (plPos.X % winX) - winX, (plPos.Y % winY) - winY ), //Upper left to right
                new Vector2( plPos.X % winX, (plPos.Y % winY) - winY ),
                new Vector2( (plPos.X % winX) + winX, (plPos.Y % winY) - winY ),

                new Vector2( (plPos.X % winX) - winX, plPos.Y % winY ), //Center left to right
                new Vector2( plPos.X % winX, plPos.Y % winY ),
                new Vector2( (plPos.X % winX) + winX, plPos.Y % winY ),

                new Vector2( (plPos.X % winX) - winX, (plPos.Y % winY)  + winY), //Down left to right
                new Vector2( plPos.X % winX, (plPos.Y % winY) + winY ),
                new Vector2( (plPos.X % winX) + winX, (plPos.Y % winY) + winY ),
            };

            for (int i = 0; i < renderPos.Count; i++)
            {
                //Left for future without bugs
                //if(mapBounds.Y - canvas.Height - renderPos[i].Y < plAbsPos.Y)
                    spriteBatch.Draw(canvas, renderPos[i], null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }
        }
    }
}
