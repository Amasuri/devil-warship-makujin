using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// Defines rules that every drawing class should follow.
    /// </summary>
    abstract public class ADrawingScreen
    {
        abstract public void Draw(DevimakuGame game, SpriteBatch spriteBatch);

        abstract public void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys);

        /// <summary>
        /// Wrapper method for quicker draw.
        /// </summary>
        protected void DrawTexture(SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, int scale, SpriteEffects effects = SpriteEffects.None)
        {
            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, scale, effects, 0.0f);
        }
    }
}
