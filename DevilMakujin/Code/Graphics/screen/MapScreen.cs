using DevilMakujin.Code.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static DevilMakujin.Code.Entity.Bullet;
using static DevilMakujin.Code.Entity.PlayerEquipInfo;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// The thing changes after each stage
    /// </summary>
    public class MapScreen : ADrawingScreen
    {
        private readonly Tuple<string, ShieldType, BulletType> itemSetMedium;
        private readonly Tuple<string, BulletType, BulletType> itemSetInner;
        private readonly Tuple<string, ShieldType, BulletType> itemSetBoss;

        private readonly Texture2D[] mapBg;

        public bool hasTriggeredAdvance;
        public bool hasSelectedWeapon;
        private int blackoutAlpha;

        public MapScreen(DevimakuGame game)
        {
            hasTriggeredAdvance = false;
            hasSelectedWeapon = false;

            itemSetMedium = new Tuple<string, ShieldType, BulletType>
            (
                "Z. Uranus Protection Field\nX. Vulcanus Driver",
                ShieldType.Field, BulletType.SpeedBlaster
            );
            itemSetInner = new Tuple<string, BulletType, BulletType>
            (
                "Z. Vulcanus Driver\nX. Saturnus Cannon",
                BulletType.SpeedBlaster, BulletType.Blaster
            );
            itemSetBoss = new Tuple<string, ShieldType, BulletType>
            (
                "Z. Uranus Protection Field\nX. Neptunus Longshot Blaster",
                ShieldType.Field, BulletType.LongBlaster
            );

            mapBg = new Texture2D[4];
            for (int i = 1; i < 5; i++)
                mapBg[i - 1] = game.Content.Load<Texture2D>("gui/map" + i);

            blackoutAlpha = 0;
        }

        public override void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            DrawBlackScreen(spriteBatch, game.GraphicsDevice);

            if (blackoutAlpha >= 255)
            {
                if (hasSelectedWeapon)
                    DrawContinuationScreen(spriteBatch);
                else
                    DrawWeaponSelectionScreen(spriteBatch);
            }
        }

        public override void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {
            hasTriggeredAdvance = false;
            if (blackoutAlpha < 255)
                return;

            if (hasSelectedWeapon)
            {
                if (Controls.CheckForSinglePress(keys, oldKeys, Controls.accept))
                {
                    //Resetting things for the next map screen
                    hasTriggeredAdvance = true;
                    hasSelectedWeapon = false;
                    if (StageManager.CurrentStage + 1 >= StageManager.Stage.Center)
                        game.musicPlayer.PlayBossPhraseFate();

                    blackoutAlpha = 0;
                }

                if (StageManager.CurrentStage + 1 >= StageManager.Stage.Center)
                {
                    game.musicPlayer.PlaySound(Music.SoundEvent.MapBossWarning);
                }
            }
            else
            {
                if (Controls.CheckForSinglePress(keys, oldKeys, Controls.accept) || Controls.CheckForSinglePress(keys, oldKeys, Controls.cancel))
                {
                    hasSelectedWeapon = true;
                    HandleWeaponSelection(keys, oldKeys);
                }
            }
        }

        private void DrawWeaponSelectionScreen(SpriteBatch spriteBatch)
        {
            string text = "Loot! Pick one item:\n" + StringForCurrentLevel();

            int scale = GlobalDrawArranger.Scale;
            Vector2 len = GlobalDrawArranger.mapFont.MeasureString(text);
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth * scale, DevimakuGame.defGameHeight * scale);
            Vector2 pos = new Vector2((winSize.X / 2) - (len.X / 2), (winSize.Y / 2) - (len.Y / 2));

            spriteBatch.DrawString(GlobalDrawArranger.mapFont, text, pos, Color.White);
        }

        private void DrawBlackScreen(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            int scale = GlobalDrawArranger.Scale;
            int winX = DevimakuGame.defGameWidth * scale;
            int winY = DevimakuGame.defGameHeight * scale;
            var rect = new Rectangle(0, 0, winX, winY);

            spriteBatch.Draw(Shader.Fill(graphics, GlobalDrawArranger.pixel, new Color(0, 0, 0, blackoutAlpha) ), rect, Color.White);

            if (blackoutAlpha < 255)
                blackoutAlpha += 3;
        }

        private string StringForCurrentLevel()
        {
            var stg = StageManager.CurrentStage + 1;

            if (stg == StageManager.Stage.MediumRing)
                return itemSetMedium.Item1;
            else if (stg == StageManager.Stage.InnerRing)
                return itemSetInner.Item1;
            else
                return itemSetBoss.Item1;
        }

        private void DrawContinuationScreen(SpriteBatch spriteBatch)
        {
            string text = "Forward to ";
            switch (StageManager.CurrentStage + 1)
            {
                case StageManager.Stage.MediumRing:
                    text += "medium ring!";
                    break;
                case StageManager.Stage.InnerRing:
                    text += "inner ring!";
                    break;
                case StageManager.Stage.Center:
                    text += "the boss!";
                    break;
            }

            int scale = GlobalDrawArranger.Scale;
            Vector2 len = GlobalDrawArranger.mapFont.MeasureString(text);
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth * scale, DevimakuGame.defGameHeight * scale);
            Vector2 pos = new Vector2((winSize.X / 2) - (len.X / 2), (winSize.Y / 2) - (len.Y / 2));

            if((int)StageManager.CurrentStage < mapBg.Length)
                DrawTexture(spriteBatch, mapBg[(int)StageManager.CurrentStage], Vector2.Zero, scale);

            //spriteBatch.DrawString(font, text, pos, Color.White);
        }

        private void HandleWeaponSelection(KeyboardState keys, KeyboardState oldKeys)
        {
            bool selectUpper = false;
            if (Controls.CheckForSinglePress(keys, oldKeys, Controls.accept))
                selectUpper = true;

            var stg = StageManager.CurrentStage + 1;
            if (stg == StageManager.Stage.MediumRing)
            {
                if (selectUpper)
                    PlayerEquipInfo.shield = itemSetMedium.Item2;
                else
                    PlayerEquipInfo.gun = itemSetMedium.Item3;
            }
            else if (stg == StageManager.Stage.InnerRing)
            {
                if (selectUpper)
                    PlayerEquipInfo.gun = itemSetInner.Item2;
                else
                    PlayerEquipInfo.gun = itemSetInner.Item3;
            }
            else
            {
                if (selectUpper)
                    PlayerEquipInfo.shield = itemSetBoss.Item2;
                else
                    PlayerEquipInfo.gun = itemSetBoss.Item3;
            }
        }
    }
}
