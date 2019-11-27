using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public class ShipRocketeer : GenericEnemy
    {
        private const int RAW_PREFFERED_DISTANCE = 450;
        private const float MIN_MOVE_THRESHOLD = 0.1f;
        private const float INERTIA_MULTIPLIER = 0.95f;
        private const float MAX_ROCKET_SALVO_DELAY = 5000f;

        private float CurrentDistanceThreshold => RAW_PREFFERED_DISTANCE / 2 * GlobalDrawArranger.Scale;

        private float GetDistance(Vector2 vecDiff) => Math.Abs(vecDiff.Length());

        private AiMode aiMode;
        private float FiringCooldown;

        private enum AiMode
        {
            Seek,
            Fire
        }

        public ShipRocketeer(Vector2 initPos, EntityType type = EntityType.Enemy) : base(initPos, type)
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

            if(this.aiMode == AiMode.Seek)
            {
                this.speed = new Vector2(
                        Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
                        Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
                    );

                if (GetDistance(diff) <= CurrentDistanceThreshold)
                {
                    TriggerFireMode();
                }
            }
            else if(this.aiMode != AiMode.Seek)
            {
                if (GetDistance(diff) >= CurrentDistanceThreshold * 1.3f)
                    TriggerSeekMode();

                if (Math.Abs(speed.Length()) >= MIN_MOVE_THRESHOLD)
                    this.speed = this.oldSpeed * INERTIA_MULTIPLIER;
            }

            this.oldSpeed = this.speed;
        }

        public override List<AEntity> ShootAtPlayer(Vector2 plPos)
        {
            Vector2 diff = plPos - absPos;

            if (this.aiMode == AiMode.Fire)
            {
                this.FiringCooldown -= DevimakuGame.DeltaUpdate;

                if (this.FiringCooldown <= 0f)
                {
                    this.FiringCooldown = MAX_ROCKET_SALVO_DELAY;
                    return ConstructShootList(ConstructShootVector(plPos));
                }

                return new List<AEntity>();
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
                new BulletRocket(this.absPos + new Vector2(10 * GlobalDrawArranger.Scale, 0), rType: BulletRocket.RocketType.Rocketeer),
                new BulletRocket(this.absPos - new Vector2(10 * GlobalDrawArranger.Scale, 0), rType: BulletRocket.RocketType.Rocketeer)
            };
        }

        private void TriggerSeekMode()
        {
            this.aiMode = AiMode.Seek;
        }

        private void TriggerFireMode()
        {
            this.aiMode = AiMode.Fire;
            this.FiringCooldown = 0;
        }
    }
}
