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

        /// <summary>
        /// The first argument is needed, because laser exists tied to the entity. If no entity specified, speed/angle is taken from player
        /// (who is not an entity... what was I thinking during the jam?????)
        /// </summary>
        public BulletLaser(AEntity refToCaller = null)
        {
            if (refToCaller != null)
                this.absPos = refToCaller.GetPos();
            else
                this.absPos = PlayerPhysics.PlayerAbsPos;

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
            if (refToCaller != null)
                this.absPos = refToCaller.GetPos();
            else
                this.absPos = PlayerPhysics.PlayerAbsPos;

            return this.absPos;
        }

        /// <summary>
        /// Stitches to the caller so doesn't have speed of it's own
        /// </summary>
        public override void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
            if (refToCaller != null)
                this.speed = refToCaller.GetSpeed();
            else
                this.speed = PlayerPhysics.PlayerPosDiff;
        }

        public override void UpdatePos()
        {
            ; //you do nothin you puny mortal
        }
    }
}
