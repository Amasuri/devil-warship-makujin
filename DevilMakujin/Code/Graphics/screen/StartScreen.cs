using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevilMakujin.Code.Graphics
{
    public class StartScreen : ADrawingScreen
    {
        public bool hasPressedStart;
        public int count;

        public StartScreen(DevimakuGame game)
        {
            hasPressedStart = false;
            count = -1;
        }

        public override void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            string text = count < 0 ? "Press Start" : "Ready";
            if (count > 0 && count < 75)
                text = "Forward to outer ring!";

            int scale = GlobalDrawArranger.Scale;
            Vector2 len = GlobalDrawArranger.mapFont.MeasureString(text);
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth*scale, DevimakuGame.defGameHeight*scale);
            Vector2 pos = new Vector2((winSize.X/2) - (len.X/2), (winSize.Y / 2) - (len.Y/2));

            spriteBatch.DrawString(GlobalDrawArranger.mapFont, text, pos, Color.White);
        }

        public override void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {
            if (keys.IsKeyDown(Controls.accept) && count == -1)
            {
                count = 150;
                game.musicPlayer.PlaySound(Music.SoundEvent.GameStart);
            }

            if (count > 0)
                count--;
            if (count == 0)
                hasPressedStart = true;
        }
    }
}
