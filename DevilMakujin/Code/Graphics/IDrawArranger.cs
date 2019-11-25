using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderDemo.Code.Graphics
{
    /// <summary>
    /// Doesn't draw anything on it's own, but calls scene drawers based on logic.
    /// Same with interface input handling.
    /// </summary>
    public interface IDrawArranger
    {
        void CallDraws(ShaderDemo shaderDemo, SpriteBatch defaultSpriteBatch, GraphicsDevice graphicsDevice);

        void CallGuiControlUpdates(ShaderDemo game);
    }
}
