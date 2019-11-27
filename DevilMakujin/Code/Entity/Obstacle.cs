using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class Obstacle : AEntity
    {
        public Obstacle(Vector2 initPos)
        {
            this.absPos = initPos;
            this.speed = Vector2.Zero;
            this.type = EntityType.Projectile;
            this.img = asteroidImg[0];
            this.faction = EntityFaction.Player;
        }
    }
}
