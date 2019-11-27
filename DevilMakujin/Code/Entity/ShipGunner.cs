using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class ShipGunner : GenericEnemy
    {
        private const int RAW_PREFFERED_DISTANCE = 370;
        private const float MINIMAL_MOVE_THRESHOLD = 0.1f;
        private const float INERTIA_MULTIPLIER = 0.95f;
        private const float MAX_FIRE_TIMER = 5000f;
        private const float MAX_RETREAT_TIMER = 3000f;

        private float CurrentDistanceThreshold => RAW_PREFFERED_DISTANCE / 2 * GlobalDrawArranger.Scale;

        private AiMode aiMode;
        private float currentRetreatTimer;
        private float currentFireTimer;

        public enum AiMode
        {
            Seek,
            Fire,
            Retreat
        }

        public ShipGunner(Vector2 initPos, EntityType type = EntityType.Enemy) : base(initPos, type)
        {
            this.absPos = initPos;
            this.speed = Vector2.Zero;
            this.type = type;
            this.img = pirateImg[DevimakuGame.Rand.Next(pirateImg.Count)];
            this.faction = EntityFaction.Enemies;
            this.aiMode = AiMode.Seek;

            this.shootTime = 0;
        }

        override public void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            Vector2 diff = plPos - absPos;

            if (this.aiMode != AiMode.Retreat)
            {
                if (Math.Abs(diff.Length()) <= CurrentDistanceThreshold)
                {
                    if (this.aiMode == AiMode.Seek)
                        TriggerFireAiMode();

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
            else if (this.aiMode == AiMode.Retreat)
            {
                this.speed = new Vector2(
                        Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
                        Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
                    ) * -2;

                this.currentRetreatTimer -= DevimakuGame.DeltaUpdate;
                if (this.currentRetreatTimer <= 0f)
                    TriggerSeekAiMode();
            }

            this.oldSpeed = this.speed;
        }

        public override List<AEntity> ShootAtPlayer(Vector2 plPos)
        {
            Vector2 diff = plPos - absPos;

            if (this.aiMode == AiMode.Fire)
            {
                this.currentFireTimer -= DevimakuGame.DeltaUpdate;
                if (this.currentFireTimer <= 0f || Math.Abs(diff.Length()) >= CurrentDistanceThreshold * 2)
                    this.TriggerRetreatAiMode();

                return base.ShootAtPlayer(plPos);
            }
            else
            {
                return new List<AEntity>();
            }
        }

        private void TriggerFireAiMode()
        {
            this.aiMode = AiMode.Fire;
            this.currentFireTimer = MAX_FIRE_TIMER;
        }

        private void TriggerRetreatAiMode()
        {
            this.aiMode = AiMode.Retreat;
            this.currentRetreatTimer = MAX_RETREAT_TIMER;
        }

        private void TriggerSeekAiMode()
        {
            this.aiMode = AiMode.Seek;
        }
    }
}
