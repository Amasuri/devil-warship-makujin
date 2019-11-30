using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public class ShipSniper : GenericEnemy
    {
        private const int RAW_PREFFERED_DISTANCE = 600;
        private const float MINIMAL_MOVE_THRESHOLD = 0.1f;
        private const float INERTIA_MULTIPLIER = 0.95f;
        private const float MAX_PREPARE_TIMER = 3000f;

        private float CurrentDistanceThreshold => RAW_PREFFERED_DISTANCE / 2 * GlobalDrawArranger.Scale;

        private AiMode aiMode;
        private float PrepareTimer;
        private float RestTimer;

        public enum AiMode
        {
            Seek,
            Prepare,
            Fire,
            Rest
        }

        public ShipSniper(Vector2 initPos, EntityType type = EntityType.Enemy) : base(initPos, type)
        {
            this.absPos = initPos;
            this.speed = Vector2.Zero;
            this.type = type;
            this.img = enemyShipImg[1];
            this.faction = EntityFaction.Enemies;
            this.aiMode = AiMode.Seek;

            this.PrepareTimer = MAX_PREPARE_TIMER;
            this.RestTimer = MAX_PREPARE_TIMER;

            this.shootTime = 0;
        }

        override public void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            Vector2 diff = plPos - absPos;

            //General logic (state machine flow & move updating)
            if (this.aiMode == AiMode.Seek)
            {
                if (Math.Abs(diff.Length()) <= CurrentDistanceThreshold)
                {
                    TriggerPrepareAiMode();

                    if (Math.Abs(speed.Length()) >= MINIMAL_MOVE_THRESHOLD)
                        this.speed = this.oldSpeed * INERTIA_MULTIPLIER;
                }
                else
                {
                    this.speed = new Vector2(
                        Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
                        Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
                    );
                }
            }
            else if (this.aiMode != AiMode.Seek && this.aiMode != AiMode.Rest)
            {
                if (Math.Abs(diff.Length()) >= CurrentDistanceThreshold * 1.5f)
                    TriggerSeekAiMode();
            }

            //Get down once not seeking
            if(this.aiMode != AiMode.Seek)
            {
                if (Math.Abs(speed.Length()) >= MINIMAL_MOVE_THRESHOLD)
                    this.speed = this.oldSpeed * INERTIA_MULTIPLIER;
            }

            //Update timers
            if(this.aiMode == AiMode.Prepare)
            {
                this.PrepareTimer -= DevimakuGame.DeltaUpdate;
                if (this.PrepareTimer <= 0f)
                    TriggerFireAiMode();
            }
            else if (this.aiMode == AiMode.Rest)
            {
                this.RestTimer -= DevimakuGame.DeltaUpdate;
                if (this.RestTimer <= 0f)
                    TriggerSeekAiMode();
            }

            this.oldSpeed = this.speed;
        }

        public override List<AEntity> ShootAtPlayer(Vector2 plPos)
        {
            Vector2 diff = plPos - absPos;

            if (this.aiMode == AiMode.Fire)
            {
                TriggerRestAiMode();

                return ConstructShootList(ConstructShootVector(plPos));
            }
            else
            {
                return new List<AEntity>();
            }
        }

        override protected List<AEntity> ConstructShootList(Vector2 shootVector)
        {
            return new List<AEntity>
            {
                new Bullet(this.absPos + new Vector2(10 * GlobalDrawArranger.Scale, 0), shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.LongBlaster),
                new Bullet(this.absPos - new Vector2(10 * GlobalDrawArranger.Scale, 0), shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.LongBlaster),
                new Bullet(this.absPos + new Vector2(5 * GlobalDrawArranger.Scale, 0), shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.LongBlaster),
                new Bullet(this.absPos - new Vector2(5 * GlobalDrawArranger.Scale, 0), shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.LongBlaster)
            };
        }

        private void TriggerRestAiMode()
        {
            this.aiMode = AiMode.Rest;
            this.RestTimer = MAX_PREPARE_TIMER;
        }

        private void TriggerPrepareAiMode()
        {
            this.aiMode = AiMode.Prepare;
            this.PrepareTimer = MAX_PREPARE_TIMER;
        }

        private void TriggerFireAiMode()
        {
            this.aiMode = AiMode.Fire;
        }

        private void TriggerSeekAiMode()
        {
            this.aiMode = AiMode.Seek;
        }
    }
}
