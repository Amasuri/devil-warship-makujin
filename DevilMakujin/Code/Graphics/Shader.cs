using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevilMakujin.Code.Graphics
{
    static public class Shader
    {
        /// <summary>
        /// Returns a contoured version of 2D texture.
        /// </summary>
        static public Texture2D Contour(GraphicsDevice graphics, Texture2D original, Color contour)
        {
            int w = original.Width;
            int h = original.Height;

            Color[] c = new Color[w*h];
            original.GetData(c);

            //Convert to grid (note the row-major order as opposed to column major - basically (y,x) vs (x,y) )
            Color[,] grid = new Color[h, w];
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                    grid[row, col] = c[(row * w) + col];
            }

            //Change the grid in four steps (since we check adjacent pixels, we have to have some space to check in every direction; so
            //to not miss anything out, we check four times:
            //(also note that we check for contour color too, since the image is changed every direction we fill, so the alphas of the original)
            //(one possible solution for this is to have four different white contour images for every directions' result and then sum them up together)
            //(but since we have no pure white sprites in the game, it's an unnecessary optimization)

            //1. Change the grid to have white contour (vertically, lefter)
            for (int row = 0; row < h-1; row++)
            {
                for (int col = 0; col < w; col++)
                    if (grid[row, col].A == 0 && grid[row + 1, col].A != 0 && grid[row + 1, col] != contour)
                        grid[row, col] = contour;
            }

            //2. Change the grid to have white contour (vertically, righter)
            for (int row = 1; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                    if (grid[row, col].A == 0 && grid[row - 1, col].A != 0 && grid[row - 1, col] != contour)
                        grid[row, col] = contour;
            }

            //3. Change the grid to have white contour (horizontally, downer)
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w - 1; col++)
                    if (grid[row, col].A == 0 && grid[row, col + 1].A != 0 && grid[row, col + 1] != contour)
                        grid[row, col] = contour;
            }

            //4. Change the grid to have white contour (horizontally, upper)
            for (int row = 0; row < h; row++)
            {
                for (int col = 1; col < w; col++)
                    if (grid[row, col].A == 0 && grid[row, col - 1].A != 0 && grid[row, col - 1] != contour)
                        grid[row, col] = contour;
            }

            //Convert back
            Color[] cData = new Color[w*h];
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                    cData[(row * w) + col] = grid[row, col];
            }

            Texture2D shaded = new Texture2D(graphics, w, h);
            shaded.SetData(cData);

            return shaded;
        }

        static public Texture2D Negative(GraphicsDevice graphics, Texture2D original)
        {
            int w = original.Width;
            int h = original.Height;

            Color[] cData = new Color[w * h];
            original.GetData(cData);

            for (int i = 0; i < cData.Length; i++)
            {
                if(cData[i].A > 0)
                cData[i] = new Color(255 - cData[i].R, 255 - cData[i].G, 255 - cData[i].B, cData[i].A);
            }

            Texture2D shaded = new Texture2D(graphics, w, h);
            shaded.SetData(cData);

            return shaded;
        }

        static public Texture2D Silhouette(GraphicsDevice graphics, Texture2D original, Color silhouetteColor)
        {
            int w = original.Width;
            int h = original.Height;

            Color[] cData = new Color[w * h];
            original.GetData(cData);

            for (int i = 0; i < cData.Length; i++)
            {
                if (cData[i].A > 0)
                    cData[i] = new Color(silhouetteColor.R, silhouetteColor.G, silhouetteColor.B, cData[i].A);
            }

            Texture2D shaded = new Texture2D(graphics, w, h);
            shaded.SetData(cData);

            return shaded;
        }

        static public Texture2D Fill(GraphicsDevice graphics, Texture2D original, Color fillColor)
        {
            int w = original.Width;
            int h = original.Height;

            Color[] cData = new Color[w * h];
            original.GetData(cData);

            for (int i = 0; i < cData.Length; i++)
            {
                if (cData[i].A > 0)
                    cData[i] = new Color(fillColor.R, fillColor.G, fillColor.B, fillColor.A);
            }

            Texture2D shaded = new Texture2D(graphics, w, h);
            shaded.SetData(cData);

            return shaded;
        }
    }
}
