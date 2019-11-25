using DevilMakujin.Code.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Graphics
{
    public class ExplosionDrawer
    {
        /// <summary>
        /// Vector2 defines position. Int defines time left in milliseconds.
        /// A certain frame is displayed once ms is between some values, for every pair.
        /// Once the value is zero or less, the pair is removed.
        /// </summary>
        private readonly Dictionary<Vector2, int> explosions;

        private readonly Texture2D explosionAtlas;

        private const int oneFrameTime = 50;
        private const int frames = 10;
        private const int maxFrameTime = oneFrameTime * frames;

        private readonly int frameSizeX;
        private readonly int frameSizeY;

        public ExplosionDrawer(DevimakuGame game)
        {
            explosionAtlas = game.Content.Load<Texture2D>("effects/small_exp");
            explosions = new Dictionary<Vector2, int>();

            frameSizeX = explosionAtlas.Width / frames;
            frameSizeY = explosionAtlas.Height;
        }

        /// <summary>
        /// Draws all sourcerects of current frames
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, float scale, Vector2 playerPos)
        {
            foreach (var pair in explosions)
            {
                Vector2 nextDrawPos = pair.Key - playerPos;
                int currentFrame = frames - (pair.Value / oneFrameTime);
                Rectangle sourceRect = new Rectangle(currentFrame*frameSizeX, 0, frameSizeX, frameSizeY);

                spriteBatch.Draw(explosionAtlas, nextDrawPos, sourceRect, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }
        }

        /// <summary>
        /// Reduces frametime for explosions, removes unused ones
        /// </summary>
        public void Update(TimeSpan delta)
        {
            List<Vector2> deleteList = new List<Vector2>();
            foreach (Vector2 pos in new List<Vector2>(explosions.Keys)) //in c# simple value changes count as collection changeds, duh
            {
                explosions[pos] -= delta.Milliseconds;
                if (explosions[pos] <= 0)
                    deleteList.Add(pos);
            }

            foreach (var pos in deleteList)
            {
                if (explosions.ContainsKey(pos))
                    explosions.Remove(pos);
            }
        }

        /// <summary>
        /// Also checks if enemy is okay for adding (we don't want bullets to explode like enemies.. ok we do but still)
        /// </summary>
        /// <param name="pos"></param>
        public void AddPositionToDrawIfApplicable(Vector2 pos, AEntity entity)
        {
            if (entity is BossEntity || entity is Bullet)
                return;

            float scale = GlobalDrawArranger.Scale;
            Vector2 imgSize = entity.GetImgSize() * scale;
            Vector2 expSize = new Vector2(explosionAtlas.Width / frames * scale, explosionAtlas.Height * scale);
            Vector2 offset = (expSize - imgSize) / 2;

            if(!explosions.ContainsKey(pos - offset))
                explosions.Add(pos - offset, maxFrameTime + oneFrameTime - 1);
        }

        /// <summary>
        /// Removes explosion lists, so no old explosions occur in a new place
        /// </summary>
        public void OnStageClear()
        {
            this.explosions.Clear();
        }
    }
}
