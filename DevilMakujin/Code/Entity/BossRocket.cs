using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class BossRocket : GenericEnemy
    {
        public BossRocket(Vector2 initPos, EntityType type = EntityType.Enemy) : base(initPos, type)
        {
            this.img = otherLeechImg[0];
        }

        protected override List<Bullet> ConstructShootList(Vector2 shootVector)
        {
            return new List<Bullet>();
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
