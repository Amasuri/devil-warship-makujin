using DevilMakujin.Code.Entity;
using DevilMakujin.Code.Music;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// Just a sprite with some logic
    /// Not the same as a screen drawer
    /// </summary>
    public class PlayerSprite
    {
        private enum PDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight  }

        private readonly Texture2D img;

        private int shieldRechargeCounter;
        public bool IsShieldActive { get; private set; }
        public bool isCheatInvisibilityActive;

        private readonly MusicPlayer refMusic;

        /// <summary>
        /// Todo bug debug:
        /// Move this to a player class together with HP stuff
        /// </summary>
        private int invisCount;

        public PlayerSprite(DevimakuGame game)
        {
            this.img = game.Content.Load<Texture2D>("entity/player");
            this.invisCount = 0;
            this.shieldRechargeCounter = 0;
            this.IsShieldActive = true;
            this.isCheatInvisibilityActive = false;

            refMusic = game.musicPlayer;
        }

        public void Render(SpriteBatch spriteBatch, GraphicsDevice graphics, Vector2 drawOffset, float rotation, float maxSpeed, int scale)
        {
            Vector2 pos = drawOffset;

            Vector2 origin = new Vector2(img.Width / 2, img.Height / 2);

            if (isCheatInvisibilityActive)
            {
                spriteBatch.Draw(Shader.Contour(graphics, img, Color.MonoGameOrange), pos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            }
            else if (IsInvisible())
            {
                spriteBatch.Draw(Shader.Contour(graphics, img, Color.White), pos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            }
            else
            {
                if (PlayerEquipInfo.shield != PlayerEquipInfo.ShieldType.None && IsShieldActive)
                    spriteBatch.Draw(Shader.Contour(graphics, img, Color.CornflowerBlue), pos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(img, pos, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0.0f);
            }
        }

        /// <summary>
        /// Nothing controls related; things like counters and sound event invokers
        /// </summary>
        public void Update()
        {
            if (invisCount > 0)
                invisCount--;

            if (shieldRechargeCounter > 0)
            {
                shieldRechargeCounter--;
                IsShieldActive = false;
            }
            else if (!IsShieldActive)
            {
                IsShieldActive = true;
                refMusic.PlaySound(SoundEvent.ShieldUp);
            }
        }

        /// <summary>
        /// Not player rect
        /// </summary>
        /// <returns></returns>
        public Rectangle GetImgRect()
        {
            return new Rectangle(0, 0, img.Width, img.Height);
        }

        public Rectangle GetPlayerEntityRect(Vector2 absPos, int scale)
        {
            return new Rectangle((int)absPos.X, (int)absPos.Y, img.Width*scale, img.Height*scale);
        }

        public void SetInvisibility()
        {
            this.invisCount = 100;
        }

        public bool IsInvisible()
        {
            return invisCount > 0;
        }

        public void SetShieldRecharge()
        {
            this.shieldRechargeCounter = 1000;
        }
    }
}
