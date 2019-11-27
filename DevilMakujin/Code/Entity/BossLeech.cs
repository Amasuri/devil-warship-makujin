using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public class BossLeech : GenericEnemy
    {
        /// <summary> Used for different sounds for smol and big whale-ko </summary>
        public readonly bool isBig;

        public BossLeech(Vector2 initPos, EntityType type = EntityType.Enemy, bool rand = true)
            : base(initPos, type)
        {
            if (rand)
            {
                this.img = leechImg[DevimakuGame.Rand.Next(leechImg.Count - 1)]; //two leeches without 3rd
                this.speed = new Vector2(-maxSpeed, -maxSpeed);
                this.isBig = false;
            }
            else
            {
                this.speed = new Vector2(0, maxSpeed);
                this.img = leechImg[2];
                this.isBig = true;
            }

            this.speed *= 3;
            this.UpdatePos();
        }

        protected override List<AEntity> ConstructShootList(Vector2 shootVector)
        {
            return new List<AEntity>
            {
                new Bullet(this.absPos, shootVector, EntityFaction.Enemies, bulletType: Bullet.BulletType.Leech)
            };
        }
    }
}
