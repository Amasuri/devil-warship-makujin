using System;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class ShipGunner : GenericEnemy
    {
        private const int PREFFERED_DISTANCE = 300;
        private const float MINIMAL_MOVE_THRESHOLD = 0.1f;

        public ShipGunner(Vector2 initPos, EntityType type = EntityType.Enemy) : base(initPos, type)
        {
            this.absPos = initPos;
            this.speed = Vector2.Zero;
            this.type = type;
            this.img = pirateImg[DevimakuGame.Rand.Next(pirateImg.Count)];
            this.faction = EntityFaction.Enemies;

            this.shootTime = 0;
        }

        override public void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            Vector2 diff = plPos - absPos;

            if (Math.Abs(diff.Length()) <= PREFFERED_DISTANCE / 2 * GlobalDrawArranger.Scale)
            {
                if (Math.Abs(speed.Length()) >= MINIMAL_MOVE_THRESHOLD)
                    this.speed = this.oldSpeed * 0.95f;
            }
            else
            {
                this.speed = new Vector2(
                    Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
                    Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
                );
            }

            this.oldSpeed = this.speed;
        }
    }
}
