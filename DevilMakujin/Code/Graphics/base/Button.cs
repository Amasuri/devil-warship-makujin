using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// A simple class to store constant position, easier draw (depending on whether it's pressed or not) and some rect checks.
    /// </summary>
    public class Button
    {
        private readonly Texture2D normalImg;
        private readonly Texture2D overlayImg;
        private readonly Vector2 pos;
        private readonly Vector2 overlayImgOffset;

        public Button(DevimakuGame game, string texturePath, Texture2D overlayImg, Vector2 pos, Vector2 overlayImgOffset)
        {
            this.normalImg = game.Content.Load<Texture2D>(texturePath);
            this.overlayImg = overlayImg;
            this.pos = pos;
            this.overlayImgOffset = overlayImgOffset;
        }

        /// <summary>
        /// Handles drawing button on it's pos plus changing image depending on whether it's pressed or the cursor is above it.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 playerMouse, bool isPressed = false)
        {
            float scale = GlobalDrawArranger.Scale;

            spriteBatch.Draw(normalImg, pos*scale, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);

            if(this.RectContainsPoint(playerMouse) && overlayImg != null)
                spriteBatch.Draw(overlayImg, (pos * scale) - (overlayImgOffset * scale), null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        public bool RectContainsPoint(Vector2 point)
        {
            Vector2 pixelPos = pos * GlobalDrawArranger.Scale;
            Vector2 pixelSize = normalImg.Bounds.Size.ToVector2() * GlobalDrawArranger.Scale;

            Rectangle rect = new Rectangle(pixelPos.ToPoint(), pixelSize.ToPoint());

            return rect.Contains(point);
        }
    }
}
