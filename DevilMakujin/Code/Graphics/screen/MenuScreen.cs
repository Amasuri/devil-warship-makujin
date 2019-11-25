using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevilMakujin.Code.Graphics
{
    public class MenuScreen : ADrawingScreen
    {
        /// <summary>
        /// The moves between menu and draw/updates are unique to this class only, hence
        /// private (isolation of interface)
        /// </summary>
        private enum MenuStates
        {
            Default,
            Options,
        }
        private MenuStates menuState;

        private Texture2D backgr;

        private Texture2D overlayBt;
        private Button startBt;
        private Button optionsBt;
        private Button exitBt;

        private Texture2D optionsBackgr;
        private Button langBt;

        /// <summary>
        /// Hackey way to get mouse pos since ADrawingScreen doesn't allow for draw
        /// func mouse args (rightfully so - never confuse your update and draw funcs in XNA)
        /// buuut we need this one to update imgs of buttons
        /// </summary>
        private Vector2 lastMousePos;

        /// <summary>
        /// Sort of event trigger variable but due to simpleness and oneplaceness of event, it's a bool
        /// </summary>
        public bool hasTriggeredStartGame;

        public MenuScreen(DevimakuGame game)
        {
            int scale = GlobalDrawArranger.Scale;

            ReloadAssets(game);
        }

        private void LoadOptionScreenMenu(DevimakuGame game, string path)
        {
            string filePostfix = "";

            if (game.Lang == "ru")
                filePostfix = "_ru";
            else
                filePostfix = "";

            this.optionsBackgr = game.Content.Load<Texture2D>(path + "options_general" + filePostfix);
            this.langBt = new Button(game, path + "lang" + filePostfix, game.Content.Load<Texture2D>(path+"select_lang"), new Vector2(166, 206), new Vector2(2, 2));
        }

        private void LoadDefaultScreenMenu(DevimakuGame game, string path)
        {
            this.backgr = game.Content.Load<Texture2D>(path + "menu_background");
            this.overlayBt = game.Content.Load<Texture2D>(path + "select");

            string filePostfix = "";

            if (game.Lang == "ru")
                filePostfix = "_ru";
            else
                filePostfix = "";

            this.startBt = new Button(game, path + "start" + filePostfix, this.overlayBt, new Vector2(0, 31), new Vector2(0, 2));
            this.optionsBt = new Button(game, path + "options" + filePostfix, this.overlayBt, new Vector2(0, 55), new Vector2(24, 2));
            this.exitBt = new Button(game, path + "quit" + filePostfix, this.overlayBt, new Vector2(0, 79), new Vector2(48, 2));
        }

        public override void Draw(DevimakuGame game, SpriteBatch spriteBatch)
        {
            int scale = GlobalDrawArranger.Scale;

            DrawDefaultScreen(spriteBatch, scale);

            if (menuState == MenuStates.Options)
                DrawOptionsScreen(spriteBatch, scale);
        }

        private void DrawOptionsScreen(SpriteBatch spriteBatch, int scale)
        {
            spriteBatch.Draw(optionsBackgr, new Vector2(0, 180) * scale, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);

            this.langBt.Draw(spriteBatch, lastMousePos);
        }

        private void DrawDefaultScreen(SpriteBatch spriteBatch, int scale)
        {
            spriteBatch.Draw(backgr, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            this.startBt.Draw(spriteBatch, lastMousePos);
            this.optionsBt.Draw(spriteBatch, lastMousePos);
            this.exitBt.Draw(spriteBatch, lastMousePos);
        }

        public override void Update(DevimakuGame game, MouseState mouse, MouseState oldMouse, KeyboardState keys, KeyboardState oldKeys)
        {
            this.hasTriggeredStartGame = false;
            var mousePos = mouse.Position.ToVector2();

            CheckDefaultScreenActions(mouse, mousePos);

            if (this.menuState == MenuStates.Options)
                CheckOptionScreenActions(game, mouse, oldMouse, mousePos);

            this.lastMousePos = mouse.Position.ToVector2();
        }

        private void CheckOptionScreenActions(DevimakuGame game, MouseState mouse, MouseState oldMouse, Vector2 mousePos)
        {
            if (this.langBt.RectContainsPoint(mousePos) && Controls.CheckForClick(mouse, oldMouse))
            {
                game.ChangeLang();
                this.ReloadAssets(game);
            }
        }

        private void CheckDefaultScreenActions(MouseState mouse, Vector2 mousePos)
        {
            if (this.startBt.RectContainsPoint(mousePos) && mouse.LeftButton == ButtonState.Pressed)
                this.hasTriggeredStartGame = true;
            else if (this.optionsBt.RectContainsPoint(mousePos) && mouse.LeftButton == ButtonState.Pressed)
                this.menuState = MenuStates.Options;
        }

        /// <summary>
        /// Load for the first time and after the language change
        /// </summary>
        public void ReloadAssets(DevimakuGame game)
        {
            const string path = "gui/menu/";

            LoadDefaultScreenMenu(game, path);
            LoadOptionScreenMenu(game, path + "options/");
        }
    }
}
