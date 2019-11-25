using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class Bullet : AEntity
    {
        /// <summary>
        /// The first are players/shared. After that come leech's etc
        /// </summary>
        public enum BulletType
        {
            Blaster = 0,
            SpeedBlaster,
            LongBlaster,
            Leech = 10
        }

        public Bullet(Vector2 initPos, Vector2 moveVector, EntityFaction faction, EntityType type = EntityType.Projectile, BulletType bulletType = BulletType.Blaster)
        {
            if (bulletType < BulletType.Leech)
                this.img = playerBulletImg[(int)bulletType];
            else
                this.img = otherBulletImg[0];
            this.speed = moveVector;
            this.type = type;
            this.absPos = initPos;
            this.faction = faction;
        }
    }
}
