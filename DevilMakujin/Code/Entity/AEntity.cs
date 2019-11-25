using DevilMakujin.Code.Music;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public enum EntityType
    {
        Enemy,
        Projectile
    }

    /// <summary>
    /// For collisions
    /// </summary>
    public enum EntityFaction
    {
        Player,
        Enemies
    }

    abstract public class AEntity
    {
        static protected List<Texture2D> playerBulletImg;
        static protected List<Texture2D> otherBulletImg;
        static protected List<Texture2D> pirateImg;
        static protected List<Texture2D> leechImg;
        static protected List<Texture2D> otherLeechImg;
        protected Vector2 absPos;
        protected Vector2 speed;

        protected Texture2D img;

        public EntityType type;
        public EntityFaction faction;

        /// <summary>
        /// Simply add vector speed to absolute position
        /// </summary>
        virtual public void UpdatePos()
        {
            absPos += speed;
        }

        /// <summary>
        /// Changes in vector speed
        /// </summary>
        virtual public void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
        }

        /// <summary>
        /// Should be the same for every entity
        /// </summary>
        virtual public void Render(SpriteBatch spriteBatch, Vector2 plPos, int scale, bool facingPlayer = false, float maxSpeed = 0.0f)
        {
            float rotation = Utils.AngleFromSpeed(this.speed, maxSpeed); //todo move rotation variables into PlayerPhysics

            Vector2 origin = new Vector2(img.Width / 2, img.Height / 2);
            spriteBatch.Draw(img, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        virtual public Rectangle GetRect()
        {
            int scale = GlobalDrawArranger.Scale;

            Vector2 origin = new Vector2(img.Width, img.Height);
            return new Rectangle
            (
                (int)((int)absPos.X - origin.X),
                (int)((int)absPos.Y - origin.Y),
                img.Width*scale, img.Height*scale
            );
        }

        virtual public Vector2 GetPos()
        {
            return absPos;
        }

        virtual public Vector2 GetSpeed()
        {
            return speed;
        }

        virtual public void PlaySounds(MusicPlayer music)
        {
        }

        virtual public Vector2 GetImgSize()
        {
            return new Vector2(img.Width, img.Height);
        }

        /// <summary>
        /// Initializes shared assets
        /// </summary>
        static public void InitSharedAssets(DevimakuGame game)
        {
            playerBulletImg = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("entity/bullet_medium"),
                game.Content.Load<Texture2D>("entity/bullet_small"),
                game.Content.Load<Texture2D>("entity/bullet_long"),
            };

            otherBulletImg = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("entity/bullet_minion"),
            };

            pirateImg = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("entity/pirate1"),
                game.Content.Load<Texture2D>("entity/pirate2"),
                game.Content.Load<Texture2D>("entity/pirate3"),
            };

            leechImg = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("entity/minion1"),
                game.Content.Load<Texture2D>("entity/minion2"),
                game.Content.Load<Texture2D>("entity/minion3"),
            };

            otherLeechImg = new List<Texture2D>
            {
                game.Content.Load<Texture2D>("entity/minion_rocket")
            };
        }
    }
}
