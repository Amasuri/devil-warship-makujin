using DevilMakujin.Code.Music;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DevilMakujin.Code.Entity
{
    public class BossEntity : GenericEnemy
    {
        private readonly Texture2D meat;
        private readonly List<Texture2D> metals;
        private List<Texture2D> currentFlakes; //flakes can fall off, but also get back (on rebirth)
        private readonly List<Texture2D> possibleFlakes;
        private readonly Texture2D eye;

        private int hits = 200; //the only being with hit points
        private readonly int maxHits = 200;

        private bool needSpawn3rdMinion;
        private bool needSpawnRocket;

        private int phraseCount;

        public BossEntity(DevimakuGame game, Vector2 initPos, EntityType type = EntityType.Enemy)
            : base(initPos, type)
        {
            const string folder = "entity/boss/";

            //Img is core (base) which also serves as hitbox until I override getRect
            img = game.Content.Load<Texture2D>(folder+"basis");
            meat = game.Content.Load<Texture2D>(folder + "meat");
            eye = game.Content.Load<Texture2D>(folder + "eye");
            metals = new List<Texture2D>
            {
                game.Content.Load<Texture2D>(folder + "metal1"),
                game.Content.Load<Texture2D>(folder + "metal2"),
                game.Content.Load<Texture2D>(folder + "metal3"),
                game.Content.Load<Texture2D>(folder + "metal4"),
            };

            possibleFlakes = new List<Texture2D>();
            for (int i = 1; i < 19; i++)
                possibleFlakes.Add(game.Content.Load<Texture2D>(folder + "gore/gore" + i));
            currentFlakes = new List<Texture2D>(possibleFlakes);

            phraseCount = 0;
            needSpawn3rdMinion = false;
            needSpawnRocket = false;
        }

        /// <summary>
        /// The boss renders differently (since it's so complex and doesn't face player : only his eye does)
        /// </summary>
        override public void Render(SpriteBatch spriteBatch, Vector2 plPos, int scale, bool facingPlayer = false, float maxSpeed = 0.0f)
        {
            const float rotation = 0.0f;//Utils.AngleFromSpeed(this.speed, maxSpeed);
            Vector2 origin = new Vector2(img.Width / 2, img.Height / 2);

            spriteBatch.Draw(img, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(meat, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);

            foreach (var metal in metals)
            {
                origin = new Vector2(metal.Width / 2, metal.Height / 2);
                spriteBatch.Draw(metal, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            }

            foreach (var gore in currentFlakes)
            {
                origin = new Vector2(gore.Width / 2, gore.Height / 2);
                spriteBatch.Draw(gore, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            }

            origin = new Vector2(eye.Width / 2, eye.Height / 2);
            spriteBatch.Draw(eye, -plPos + absPos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
        }

        public override Rectangle GetRect()
        {
            return base.GetRect();
        }

        public override void ChangeSpeedVector(Vector2 plPos, Vector2 mapSize)
        {
        }

        public override List<Bullet> ShootAtPlayer(Vector2 plPos)
        {
            var retList = new List<Bullet>();

            if(phraseCount == 500)
            {
                List <Vector2> directions = new List<Vector2>
                {
                    new Vector2(0, 1),
                    new Vector2(0, -1),
                    new Vector2(1, 0),
                    new Vector2(-1, 0),

                    new Vector2(1, 1),
                    new Vector2(-1, -1),
                    new Vector2(1, -1),
                    new Vector2(-1, 1),
                };

                foreach (var dir in directions)
                {
                    retList.Add(new Bullet(this.absPos, dir*maxSpeed/2, EntityFaction.Enemies, bulletType: Bullet.BulletType.Leech));
                }
            }

            return retList;
        }

        public override bool CanBeDeletedAndActionOnHit()
        {
            //If shot, remove gores with some chance
            int chance = DevimakuGame.Rand.Next(100);
            if (currentFlakes.Count > 0 && chance < 20)
            {
                currentFlakes.RemoveAt(currentFlakes.Count - 1);
                needSpawnRocket = true;
            }

            hits--;
            if (hits == 60 || hits == 120 || hits == 180)
                needSpawn3rdMinion = true;
            else
                needSpawn3rdMinion = false;

            return hits <= 0;
        }

        override public void PlaySounds(MusicPlayer music)
        {
            phraseCount++;
            if (phraseCount >= 750)
            {
                phraseCount = 0;
                music.PlayRandomBossPhrase();
            }
        }

        public List<GenericEnemy> SpawnLeeches()
        {
            List<GenericEnemy> ret = new List<GenericEnemy>();

            if (phraseCount == 200)
            {
                ret.Add(new BossLeech(absPos));
            }
            if (needSpawn3rdMinion)
            {
                needSpawn3rdMinion = false;
                ret.Add(new BossLeech(absPos, rand: false));
            }
            if (needSpawnRocket)
            {
                needSpawnRocket = false;
                ret.Add(new BossRocket(absPos));
            }

            return ret;
        }

        public override void UpdatePos()
        {
            //Doesn't move
        }

        public override void ReverseSpeedAndUpdatePos(Vector2 anotherEnemyPos)
        {
            //doesnt't jumble
        }

        public bool HitBoxContains(Vector2 pos)
        {
            float scale = GlobalDrawArranger.Scale;

            return new Circle(Vector2.Zero, img.Width / 2 * scale).Contains(pos);
        }

        override public Tuple<int, int> GetMaxToCurrentHp()
        {
            return new Tuple<int, int>(maxHits, hits);
        }

        internal override void RestoreOriginalState()
        {
            base.RestoreOriginalState();

            this.hits = maxHits;
            this.currentFlakes = new List<Texture2D>(possibleFlakes);
        }
    }
}
