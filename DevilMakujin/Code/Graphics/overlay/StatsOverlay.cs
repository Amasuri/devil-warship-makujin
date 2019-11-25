using DevilMakujin.Code.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// Has stasts about player hp but also boss hp on boss levels
    /// </summary>
    public class StatsOverlay
    {
        private readonly Texture2D[] hits;
        private readonly Texture2D bossHpOvelay;
        private readonly Texture2D bossHpGauge;

        public StatsOverlay(DevimakuGame game)
        {
            hits = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                hits[i] = game.Content.Load<Texture2D>("gui/hit" + i);

            bossHpOvelay = game.Content.Load<Texture2D>("gui/boss_hp");
            bossHpGauge = game.Content.Load<Texture2D>("gui/hp_line");
        }

        public void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            int hp = PlayerEquipInfo.Hits;
            if (hp < 0)
                hp = 0;

            int scale = GlobalDrawArranger.Scale;
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth * scale, DevimakuGame.defGameHeight * scale);
            Vector2 pos = new Vector2(0, winSize.Y- (hits[0].Height*scale));

            DrawTexture(spriteBatch, hits[hp], pos, scale);

            if (StageManager.CurrentStage == StageManager.Stage.Center)
                DrawGauge(spriteBatch, scale, EnemySet.bossSet[0].GetMaxToCurrentHp());
        }

        private void DrawGauge(SpriteBatch spriteBatch, int scale, Tuple<int, int> tuple)
        {
            float ratio = (float)tuple.Item2 / (float)tuple.Item1;
            Rectangle hpRect = new Rectangle
            (
                0, 0,
                (int)(bossHpGauge.Width * ratio),
                bossHpGauge.Height
            );

            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth * scale, DevimakuGame.defGameHeight * scale);
            Vector2 pos = new Vector2(winSize.X- (bossHpOvelay.Width*scale), winSize.Y- (bossHpOvelay.Height*scale));

            DrawTexture(spriteBatch, bossHpOvelay, pos, scale);
            spriteBatch.Draw(bossHpGauge, pos+ (new Vector2(4, 3)*scale), hpRect, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        private void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, int scale)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }
    }
}
