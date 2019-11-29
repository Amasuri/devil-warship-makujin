using DevilMakujin.Code.Graphics;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Entity
{
    public class BulletLaser : AEntity
    {
        private AEntity refToCaller;

        private Animation anim;

        private float lifeTimeLeft;
        private const float LIFE_TIME_MAX = 2000f;

        public bool ReadyToDie => lifeTimeLeft <= 0f;

        public BulletLaser()
        {
            this.absPos = refToCaller.GetPos();
            this.speed = Vector2.Zero;
            this.lifeTimeLeft = LIFE_TIME_MAX;

            this.img = playerBulletImg[2];
            this.anim = null;
        }

        /// <summary>
        /// Stitches to the caller so doesn't have pos of it's own
        /// </summary>
        public override Vector2 GetPos()
        {
            return refToCaller.GetPos();
        }

        /// <summary>
        /// Stitches to the caller so doesn't have speed of it's own
        /// </summary>
        public override void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            this.speed = refToCaller.GetSpeed();
        }

        public override void UpdatePos()
        {
            ; //you do nothin you puny mortal
        }
    }
}
