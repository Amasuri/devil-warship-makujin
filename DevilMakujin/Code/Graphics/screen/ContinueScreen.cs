using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevilMakujin.Code.Graphics
{
    public class ContinueScreen : ADrawingScreen
    {
        private readonly string continueMessage;
        private Vector2 drawPos;

        public bool HasAcceptedDefeat { private set; get; }
        public bool HasDecidedToFight { private set; get; }

        public ContinueScreen()
        {
            continueMessage = "You fell. Accept Gokagu's guidance?\nZ: yes, X: no";
            HasAcceptedDefeat = false;
            HasDecidedToFight = false;

            int scale = GlobalDrawArranger.Scale;
            Vector2 winSize = new Vector2(DevimakuGame.defGameWidth, DevimakuGame.defGameHeight) * scale;
            Vector2 textSize = GlobalDrawArranger.endFont.MeasureString(continueMessage);

            drawPos = new Vector2((winSize.X / 2) - (textSize.X / 2), (winSize.Y / 2) - (textSize.Y / 2));
        }

        public override void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Black);

            spriteBatch.DrawString(GlobalDrawArranger.endFont, continueMessage, drawPos, Color.White);
        }

        public override void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {
            if (Controls.CheckForSinglePress(keys, oldKeys, Controls.shoot))
                HasDecidedToFight = true;

            if (Controls.CheckForSinglePress(keys, oldKeys, Controls.cancel))
                HasAcceptedDefeat = true;
        }

        public void Reset()
        {
            HasAcceptedDefeat = false;
            HasDecidedToFight = false;
        }
    }
}
