using Microsoft.Xna.Framework.Graphics;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// Interface which draw arrangers follow (those who don't draw, but call draws based on logic)
    /// </summary>
    public interface IDrawArranger
    {
        void CallDraws(DevimakuGame shaderDemo, SpriteBatch defaultSpriteBatch, GraphicsDevice graphicsDevice);

        void CallGuiControlUpdates(DevimakuGame game);
    }
}
