using DevilMakujin.Code.Entity;
using DevilMakujin.Code.Music;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevilMakujin.Code.Graphics
{
    public class BattleScreen : ADrawingScreen
    {
        public bool HasEnemies { get; private set; }
        public bool IsPlayerDead { get; private set; }

        private readonly PlayerSprite playerSprite;
        private readonly ParallaxSprite parallaxSprite;
        private readonly MinimapOverlay minimapWidget;
        private readonly StatsOverlay statsWidget;
        private readonly ExplosionDrawer explosionDrawer;

        /// <summary>
        /// Does participate in position initiation in stageManager, but aside from
        /// that should be used in this class only. Due to old jam architecture it's this way now, but
        /// it will change for better later
        /// </summary>
        public readonly Vector2 playerDrawOffset;

        /// <summary> Is here temporatily. Will be moved when/if map shrink logic will be implemented. </summary>
        private const int physMapShrink = 1;

        private Rectangle mapBounds;
        private readonly Circle mapCircle;
        private readonly int MapSizeInTiles = 3;
        private readonly Texture2D blackCanvas;
        private Vector2 blackCanvasDrawOffset;

        private int shootTimer;

        /// <summary>
        /// Includes both enemies and projectiles
        /// Allies/Enemies add bullets to this list; enemies are in there too
        /// </summary>
        private readonly List<AEntity> entityList;

        public BattleScreen(DevimakuGame game)
        {
            int scale = GlobalDrawArranger.Scale;

            this.playerSprite = new PlayerSprite(game);
            this.parallaxSprite = new ParallaxSprite(game);
            this.minimapWidget = new MinimapOverlay(game);
            this.statsWidget = new StatsOverlay(game);
            this.explosionDrawer = new ExplosionDrawer(game);

            playerDrawOffset = new Vector2
            (
                (DevimakuGame.defGameWidth * scale / 2) - (this.playerSprite.GetImgRect().Width * scale / 2),
                (DevimakuGame.defGameHeight * scale / 2) - (this.playerSprite.GetImgRect().Height * scale / 2)
            );

            entityList = new List<AEntity>();

            int mapTileSize = DevimakuGame.defGameWidth * scale; //Ludum42Game.defGameWidth * scale;
            mapBounds = new Rectangle
            (
                -(mapTileSize * MapSizeInTiles)/2,
                -(mapTileSize * MapSizeInTiles)/2,
                mapTileSize * MapSizeInTiles,
                mapTileSize * MapSizeInTiles
            );

            mapCircle = new Circle
            (
                mapBounds.Center.ToVector2(),
                mapBounds.Height/2
            );

            PlayerPhysics.InitPosOnStage(StageManager.CurrentStage, playerDrawOffset);

            //Bordering
            blackCanvas = new Texture2D(game.GraphicsDevice, mapBounds.Width + (mapTileSize * 2), mapBounds.Height + (mapTileSize * 2));
            Color[] blackCanvasColors = new Color[blackCanvas.Height * blackCanvas.Width];
            for (int i = 0; i < blackCanvasColors.Length; i++)
            {
                int diffX = (int)((blackCanvas.Width - (mapCircle.Radius * 2)) / 2);
                int diffY = (int)((blackCanvas.Height - (mapCircle.Radius * 2)) / 2);
                blackCanvasDrawOffset = new Vector2(diffX, diffY);

                int x = (i / blackCanvas.Width) - diffX; //int x = i / blackCanvas.Width;
                int y = (i % blackCanvas.Height) - diffY; //int y = i % blackCanvas.Height;

                x += mapBounds.X;// - Math.Abs(mapBounds.X / 3);
                y += mapBounds.X;// - Math.Abs(mapBounds.X / 3); //it's thee same

                if (!mapCircle.Contains(new Vector2(x, y)))
                {
                    if (mapCircle.GetDistanceFromCenterInRadii(new Vector2(x, y)) <= 1.05f)
                        blackCanvasColors[i] = new Color(0, 0, 0, 100);
                    else if (mapCircle.GetDistanceFromCenterInRadii(new Vector2(x, y)) <= 1.10f)
                        blackCanvasColors[i] = new Color(0, 0, 0, 175);
                    else
                        blackCanvasColors[i] = Color.Black;
                }
                else
                {
                    blackCanvasColors[i] = new Color(0, 0, 0, 0);
                }
            }
            blackCanvas.SetData<Color>(blackCanvasColors);

            //Shoot
            shootTimer = 0;
            IsPlayerDead = false;

            //Initial addition
            StageManager.SetStartingStage(this);
        }

        public override void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            int scale = GlobalDrawArranger.Scale;

            var playerAbsPosCopy = PlayerPhysics.playerAbsPos;
            var playerSpeedCopy = PlayerPhysics.playerPosDiff;

            //Background
            this.parallaxSprite.Render(spriteBatch, scale, playerAbsPosCopy, mapBounds); //+playerDrawOffset*2 doesn't make sense since it's not offsetplayer draw (but still here in case of the offsetting bugs)

            //Entities + borders
            this.DrawEntityList(spriteBatch, scale, playerAbsPosCopy);
            this.playerSprite.Render(spriteBatch, game.GraphicsDevice, playerDrawOffset, PlayerPhysics.GetRotationForDraw(Controls.mode), PlayerPhysics.GetMaxPossibleSpeed(Controls.mode), scale);
            this.FillBordersWithBlack(spriteBatch, scale, playerAbsPosCopy + playerDrawOffset);
            this.explosionDrawer.Draw(spriteBatch, scale, playerAbsPosCopy);

            //Widgets
            this.minimapWidget.Draw(spriteBatch, scale, mapBounds, playerAbsPosCopy + playerDrawOffset, this.GetEnemyPos());
            this.statsWidget.Draw(game, spriteBatch);

            //Debug
            //this.DebugBorderDraws(spriteBatch, scale, playerAbsPosCopy + playerDrawOffset);
        }

        public override void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {
            this.playerSprite.Update();
            this.PlayLevelSfx(game.musicPlayer);
            this.PlayerInput(keys, oldKeys, game.musicPlayer);
            this.UpdateEntityList(game.musicPlayer);
            this.explosionDrawer.Update(DevimakuGame.delta);
            HasEnemies = this.CheckIfHasImportantEnemiesLeft();

            //this.TightenBounds(); todo

            //Debug
            if (Controls.CheckForSinglePress(keys, oldKeys, Controls.mute))
                MediaPlayer.Volume = 0.0f;
        }

        internal void RedeemPlayer()
        {
            //Kinda shows how fragmented player data currently is
            this.IsPlayerDead = false;
            PlayerEquipInfo.HealBetweenStages();
            PlayerPhysics.InitPosOnStage(StageManager.CurrentStage, playerDrawOffset);

            //Delete olds to avoid duplicates, then re-add new ones
            ClearEntityList();
            StageManager.AddEnemiesOnCurrentStage(this);

            //Restores enemy states (because when you add, you add ref links, so it's compendium enemies who act)
            foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
            {
                enemy.RestoreOriginalState();
            }
        }

        private void DebugBorderDraws(SpriteBatch spriteBatch, int scale, Vector2 vector2)
        {
            Vector2 center = mapCircle.Center - PlayerPhysics.playerAbsPos;
            Vector2 left = mapCircle.Center + new Vector2(mapCircle.Radius, 0) - PlayerPhysics.playerAbsPos;
            Vector2 right = mapCircle.Center + new Vector2(-mapCircle.Radius, 0) - PlayerPhysics.playerAbsPos;

            spriteBatch.Draw(GlobalDrawArranger.pixel, center, null, Color.Red, 0.0f, Vector2.Zero, 10.0f, SpriteEffects.None, 0.0f);
        }

        private void FillBordersWithBlack(SpriteBatch spriteBatch, int scale, Vector2 playerAbsPos)
        {
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth, DevimakuGame.defGameHeight) * scale;
            Vector2 mapOffset2 = (new Vector2(mapBounds.X, mapBounds.Y) *-1) + playerAbsPos + blackCanvasDrawOffset;
            Rectangle sourceRect = new Rectangle(mapOffset2.ToPoint(),  winSize.ToPoint());

            spriteBatch.Draw(blackCanvas, Vector2.Zero, sourceRect, Color.White);

            /* Old code, which draws the whole huge canvas, not it's on-screen part
             *
             * Vector2 mapOffset = new Vector2(mapBounds.X, mapBounds.Y) - playerAbsPos;
             * spriteBatch.Draw(blackCanvas, mapOffset-blackCanvasDrawOffset, Color.White);
            */
        }

        private List<Vector2> GetEnemyPos()
        {
            var ret = new List<Vector2>();

            foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
                ret.Add(enemy.GetPos());

            return ret;
        }

        private bool CheckIfHasImportantEnemiesLeft()
        {
            bool has = false;
            bool hasBoss = false;

            //General rule is to simply whip everyone
            foreach (AEntity entity in entityList)
            {
                if (entity is GenericEnemy || entity is BossEntity)
                {
                    has = true;

                    if(entity is BossEntity)
                        hasBoss = true;
                }
            }

            //On boss stage though you'd have to kill only the boss
            if (StageManager.CurrentStage == StageManager.Stage.Center && !hasBoss)
                has = false;

            return has;
        }

        private void PlayLevelSfx(MusicPlayer musicPlayer)
        {
            if(Math.Abs(mapCircle.GetDistanceFromCenterInRadii(PlayerPhysics.playerAbsPos + (playerDrawOffset * 2))) >= 0.9f)
            {
                musicPlayer.PlaySound(SoundEvent.NearingVoid);
            }
        }

        public void AddEnemies(List<GenericEnemy> enemies)
        {
            foreach (var enemy in enemies)
                entityList.Add(enemy);
        }

        private void DrawEntityList(SpriteBatch spriteBatch, int scale, Vector2 playerAbsPos)
        {
            var maxSpeed = PlayerPhysics.GetMaxPossibleSpeed(Controls.mode);

            foreach (var entity in entityList)
            {
                bool isFacing = false;
                if (entity is GenericEnemy)
                    isFacing = true;

                entity.Render(spriteBatch, playerAbsPos, scale, isFacing, maxSpeed);
            }
        }

        private void UpdateEntityList(MusicPlayer music)
        {
            //Updating collisions
            List<AEntity> deleteList = new List<AEntity>();
            foreach (Bullet bullet in entityList.Where(bul => bul is Bullet))
            {
                foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
                {
                    if (bullet.faction == EntityFaction.Enemies)
                        break;

                    if (bullet.GetRect().Intersects(enemy.GetRect()))
                    {
                        if (!deleteList.Contains(enemy) && enemy.CanBeDeletedAndActionOnHit())
                        {
                            deleteList.Add(enemy);
                        }
                        if (!deleteList.Contains(bullet))
                            deleteList.Add(bullet);

                        //todo probably can break after one addition?
                    }
                }
            }

            //Updating player hitboxes
            foreach (Bullet bullet in entityList.Where(bul => bul is Bullet && bul.faction == EntityFaction.Enemies))
            {
                if (playerSprite.IsInvisible())
                    break;

                if (bullet.GetRect().Intersects(playerSprite.GetPlayerEntityRect(PlayerPhysics.playerAbsPos + playerDrawOffset, GlobalDrawArranger.Scale)))
                {
                    IsPlayerDead = PlayerEquipInfo.HitAndCheckDead(playerSprite, music);
                    if (!deleteList.Contains(bullet))
                    {
                        deleteList.Add(bullet);
                    }
                }
            }

            //Updating player collision with enemies
            foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
            {
                if (enemy.GetRect().Intersects(playerSprite.GetPlayerEntityRect(PlayerPhysics.playerAbsPos + playerDrawOffset, GlobalDrawArranger.Scale)))
                {
                    PlayerPhysics.RevertSpeed();
                    if (playerSprite.IsInvisible())
                        break;

                    IsPlayerDead = PlayerEquipInfo.HitAndCheckDead(playerSprite, music);

                    if (enemy is BossRocket && !deleteList.Contains(enemy))
                        deleteList.Add(enemy);
                }
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Todo: if you will do HP, then delete only bullets
            //As for enemies, make a blank AEntity.Interact() and in enemies override that with depleting HP
            //Then make a predicate .Where(enemy => enemy.hp <= 0) add to delete list
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //Deleting the collisioned bullets/enemies
            foreach (var entity in deleteList)
            {
                explosionDrawer.AddPositionToDrawIfApplicable(entity.GetPos(), entity);
                music.PlayEventOnEntityDeath(entity);
                entityList.Remove(entity);
            }

            //Deleting extra bullets
            List<AEntity> bulletDeleteList = new List<AEntity>();
            foreach (Bullet bul in entityList.Where(bul => bul is Bullet))
            {
                Vector2 pos = bul.GetPos();
                if (Math.Abs(pos.X) >= mapBounds.Width * 2 || Math.Abs(pos.Y) >= mapBounds.Height * 2)
                    bulletDeleteList.Add(bul);
            }
            foreach (var bul in bulletDeleteList)
                entityList.Remove(bul);

            //Updating position/sounds/misc functions of every entity
            List<GenericEnemy> addList = new List<GenericEnemy>();
            foreach (var entity in entityList)
            {
                entity.ChangeSpeedVector(PlayerPhysics.playerAbsPos + playerDrawOffset, mapBounds.Size.ToVector2());
                entity.UpdatePos();
                entity.PlaySounds(music);

                if(entity is BossEntity refEntity) //C# 7.0 power: auto-cast
                {
                    var leeches = refEntity.SpawnLeeches();

                    foreach (var leech in leeches)
                    {
                        if(!addList.Contains(leech))
                            addList.Add(leech);
                    }
                }
            }
            this.AddEnemies(addList);

            //Making so enemies don't collide
            foreach (GenericEnemy enemy1 in entityList.Where(enemy => enemy is GenericEnemy))
            {
                foreach (GenericEnemy enemy2 in entityList.Where(enemy => enemy is GenericEnemy))
                {
                    if (enemy1.GetRect().Intersects(enemy2.GetRect()))
                    {
                        if (enemy1 != enemy2)
                        {
                            //enemy1.ReverseSpeedAndUpdatePos();
                            enemy2.ReverseSpeedAndUpdatePos(enemy1.GetPos());
                        }
                    }
                }
            }

            //Making enemies try to avoid bullets
            foreach (Bullet bullet in entityList.Where(bul => bul is Bullet && bul.faction == EntityFaction.Player))
            {
                foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
                {
                    enemy.TryAvoidBullet(bullet, mapBounds.Size.ToVector2());
                }
            }

            //Updating enemy shootings
            List<Bullet> shootList = new List<Bullet>();
            foreach (GenericEnemy enemy in entityList.Where(enemy => enemy is GenericEnemy))
            {
                List<Bullet> nextShotListAdd = enemy.ShootAtPlayer(PlayerPhysics.playerAbsPos + playerDrawOffset);
                foreach (var item in nextShotListAdd)
                    shootList.Add(item);
            }

            //Adding every enemy's bullets as projectiles from shooting list
            foreach (Bullet bul in shootList)
                entityList.Add(bul);
        }

        private void PlayerInput(KeyboardState keys, KeyboardState oldKeys, MusicPlayer music)
        {
            //Mwuhaha
            if (Controls.CheckForInvisibilityCheat(keys))
            {
                music.PlayBossPhraseFate();
                playerSprite.isCheatInvisibilityActive = true;
            }

            //Let physics class handle input, positions etc. recieved from this class (handling differs on control modes)
            PlayerPhysics.ChangePlayerSpeedBasedOnMode(keys, oldKeys, Controls.mode);

            Controls.DebugChangeControlScheme(keys, oldKeys);

            //Since map is (currently) in this class, it's also responsible for border checking
            this.CheckIfPlayerAtBounds(music);

            PlayerShooting(keys, oldKeys, music);

            //Debug input
            //if (Controls.CheckForInput(keys, oldKeys, Controls.debugSwitch))
            //{
            //    PlayerEquipInfo.gun++;
            //    if (PlayerEquipInfo.gun > Bullet.BulletType.LongBlaster)
            //        PlayerEquipInfo.gun = Bullet.BulletType.Blaster;
            //}
        }

        private void PlayerShooting(KeyboardState keys, KeyboardState oldKeys, MusicPlayer music)
        {
            //Inscrease shooting timer
            shootTimer++;
            if (shootTimer > Int32.MaxValue - 5)
                shootTimer = 0;

            //Get shooting direction as a vector of -1..1 (to be later multiplied by bullet speed)
            var shotDirection = PlayerPhysics.GetShootingDirection(Controls.mode);

            //Two different input types for various guns
            var gun = PlayerEquipInfo.gun;
            bool doShoot = shootTimer % PlayerEquipInfo.GetShootTime(gun) == 0;
            if (keys.IsKeyDown(Controls.shoot) && PlayerPhysics.playerPosDiff != Vector2.Zero && doShoot)
            {
                music.PlaySound(gun);

                switch (gun)
                {
                    case Bullet.BulletType.SpeedBlaster:
                        Vector2 leftDirecton = new Vector2(shotDirection.X - 0.03f, shotDirection.Y - 0.03f);
                        Vector2 RightDirecton = new Vector2(shotDirection.X + 0.03f, shotDirection.Y + 0.03f);

                        this.entityList.Add(new Bullet(playerDrawOffset + PlayerPhysics.playerAbsPos, shotDirection * PlayerEquipInfo.GetEquippedGunSpeed(), EntityFaction.Player, bulletType: gun));
                        this.entityList.Add(new Bullet(playerDrawOffset + PlayerPhysics.playerAbsPos, leftDirecton * PlayerEquipInfo.GetEquippedGunSpeed(), EntityFaction.Player, bulletType: gun));
                        this.entityList.Add(new Bullet(playerDrawOffset + PlayerPhysics.playerAbsPos, RightDirecton * PlayerEquipInfo.GetEquippedGunSpeed(), EntityFaction.Player, bulletType: gun));
                        break;

                    default:
                        this.entityList.Add(new Bullet(playerDrawOffset + PlayerPhysics.playerAbsPos, shotDirection * PlayerEquipInfo.GetEquippedGunSpeed(), EntityFaction.Player, bulletType: gun));
                        break;
                }
            }
        }

        private void CheckIfPlayerAtBounds(MusicPlayer music)
        {
            Vector2 pos = PlayerPhysics.playerAbsPos + (playerDrawOffset * 2);

            if (!mapCircle.Contains(pos))
            {
                PlayerPhysics.RevertSpeed();
                IsPlayerDead = PlayerEquipInfo.HitAndCheckDead(playerSprite, music);
            }
        }

        internal void ClearEntityList()
        {
            entityList.Clear();
            explosionDrawer.OnStageClear();
        }

        private void TightenBounds()
        {
            const int shrink = physMapShrink;
            mapBounds = new Rectangle
            (
                mapBounds.X+shrink, mapBounds.Y+shrink,
                mapBounds.Width-shrink, mapBounds.Height-shrink
            );
        }
    }
}
