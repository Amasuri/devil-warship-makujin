using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class BulletRocket : GenericEnemy
    {
        public enum RocketType
        {
            BossLeech,
            Rocketeer
        }

        public BulletRocket(Vector2 initPos, EntityType type = EntityType.Enemy, RocketType rType = RocketType.BossLeech) : base(initPos, type)
        {
            if (rType == RocketType.BossLeech)
                this.img = otherLeechImg[0];
            else
                this.img = otherLeechImg[0];
        }

        protected override List<AEntity> ConstructShootList(Vector2 shootVector)
        {
            return new List<AEntity>();
        }

        public override void ReverseSpeedAndUpdatePos(Vector2 anotherEnemyPos)
        {
        }

        public override void UpdatePos()
        {
            this.speed.Normalize();
            this.speed *= maxSpeed / 4;

            base.UpdatePos();
        }
    }
}
