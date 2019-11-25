using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Graphics
{
    public class MinimapOverlay
    {
        private Tuple<Texture2D, Texture2D, Vector2> mapVidget;
        private readonly Texture2D player;
        private readonly Texture2D anyEnemy;

        private Vector2 dotDrawOffset;

        public MinimapOverlay(DevimakuGame game)
        {
            int scale = GlobalDrawArranger.Scale;

            LoadMapImage(game, scale);

            player = new Texture2D(game.GraphicsDevice, 1, 1);
            player.SetData<Color>(new Color[] { Color.BlueViolet });
            anyEnemy = new Texture2D(game.GraphicsDevice, 1, 1) ;
            anyEnemy.SetData<Color>(new Color[] { Color.DarkRed });

            dotDrawOffset = new Vector2(mapVidget.Item1.Width, mapVidget.Item1.Height);
        }

        private void LoadMapImage(DevimakuGame game, int scale)
        {
            Vector2 drawPos = new Vector2((DevimakuGame.defGameWidth - game.Content.Load<Texture2D>("gui/minimap").Width) * scale, 0);
            mapVidget = new Tuple<Texture2D,Texture2D, Vector2>
            (
                game.Content.Load<Texture2D>("gui/minimap_background"),
                game.Content.Load<Texture2D>("gui/minimap"),
                drawPos
            );
        }

        public void Draw(SpriteBatch spriteBatch, int scale, Rectangle mapBounds, Vector2 plPos, List<Vector2> enemyPos)
        {
            //Draw background
            DrawTexture(spriteBatch, mapVidget.Item1, mapVidget.Item3, scale);

            //Note that this ratio will get updated with pass of stage time (because mapBounds are shrinking)
            var minimapToPosRatio = (new Vector2(mapVidget.Item1.Width, mapVidget.Item1.Height)*scale) / mapBounds.Size.ToVector2();

            //Draw player
            Vector2 plMapPos = plPos*minimapToPosRatio;
            DrawTexture(spriteBatch, player, mapVidget.Item3 + plMapPos + dotDrawOffset, scale);

            //Draw enemies
            this.DrawEnemyDots(spriteBatch, scale, mapBounds, minimapToPosRatio, enemyPos);

            //Draw map overlay
            DrawTexture(spriteBatch, mapVidget.Item2, mapVidget.Item3, scale);
        }

        private void DrawEnemyDots(SpriteBatch spriteBatch, int scale, Rectangle mapBounds, Vector2 minimapToPosRatio, List<Vector2> enemyPos)
        {
            foreach (var pos in enemyPos)
            {
                var nextPos = pos * minimapToPosRatio;
                DrawTexture(spriteBatch, anyEnemy, mapVidget.Item3 + nextPos + dotDrawOffset, scale);
            }
        }

        private void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, int scale)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
    }
}
