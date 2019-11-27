using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public class GenericEnemy : AEntity
    {
        protected int shootTime;
        protected const float maxSpeed = 20;

        protected readonly Vector2 startingPos;

        public GenericEnemy(Vector2 initPos, EntityType type = EntityType.Enemy)
        {
            this.absPos = initPos;
            this.startingPos = absPos;
            this.speed = Vector2.Zero;
            this.type = type;
            this.img = pirateImg[DevimakuGame.Rand.Next(pirateImg.Count)];
            this.faction = EntityFaction.Enemies;

            this.shootTime = 0;
        }

        /// <summary>
        /// Performs equations whether to shoot or not.
        /// If shoot, return list of shot bullets.
        /// If not, return empty list.
        /// </summary>
        virtual public List<Bullet> ShootAtPlayer(Vector2 plPos)
        {
            var retList = new List<Bullet>();
            shootTime++;

            if (shootTime >= DevimakuGame.Rand.Next(700*3))
            {
                Vector2 shootVector = ConstructShootVector(plPos);

                retList = this.ConstructShootList(shootVector);
                shootTime = 0;
            }

            return retList;
        }

        protected Vector2 ConstructShootVector(Vector2 plPos)
        {
            double randPrecise = DevimakuGame.Rand.NextDouble()/10;
            Vector2 shoot = (plPos - this.absPos);
            shoot.Normalize();

            return (shoot + new Vector2((float)randPrecise))*maxSpeed/2;
        }

        /// <summary>
        /// Not the same as update pos, this requires player logic
        /// </summary>
        override public void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            Vector2 diff = plPos-absPos;

            this.speed = new Vector2(
                Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
                Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
            );

            //this.speed = diff;
        }

        virtual protected List<Bullet> ConstructShootList(Vector2 shootVector)
        {
            return new List<Bullet>
            {
                new Bullet(this.absPos, shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.SpeedBlaster)
            };
        }

        public Vector2 GetPos()
        {
            return this.absPos;
        }

        virtual public bool CanBeDeletedAndActionOnHit()
        {
            return true;
        }

        virtual public void ReverseSpeedAndUpdatePos(Vector2 anotherEnemyPos)
        {
            Vector2 diff = absPos - anotherEnemyPos;
            this.absPos = absPos + diff;
        }

        virtual public Tuple<int, int> GetMaxToCurrentHp()
        {
            return new Tuple<int, int>(1, 1);
        }

        virtual public void TryAvoidBullet(Bullet bullet, Vector2 mapSize)
        {
            //Vector2 nextBulletPos = bullet.getPos() + bullet.getSpeed();
            //float moveThresholdX = Math.Abs( nextBulletPos.X - absPos.X);
            //float moveThresholdY = Math.Abs( nextBulletPos.Y - absPos.Y);
            //int scale = GlobalDrawArranger.Scale;

            //if (moveThresholdX < img.Width * scale * 3 && moveThresholdY < img.Height * scale * 3)
            //{
            //    Vector2 diff = absPos - nextBulletPos;

            //    this.speed = new Vector2
            //    (
            //        Utils.Remap(diff.X, -mapSize.X, mapSize.X, -10, 10),
            //        Utils.Remap(diff.Y, -mapSize.Y, mapSize.Y, -10, 10)
            //    );

            //    this.UpdatePos();
            //}
        }

        internal virtual void RestoreOriginalState()
        {
            this.absPos = startingPos;
            this.shootTime = 0;
            this.speed = Vector2.Zero;
        }
    }
}
