using DevilMakujin.Code.Graphics;
using DevilMakujin.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DevilMakujin.Code
{
    /// <summary>
    /// Calls other draw things. Doesn't draw anything on it's own.
    /// </summary>
    public class GlobalDrawArranger : IDrawArranger
    {
        private readonly BattleScreen battleScreen;
        private readonly MenuScreen menuScreen;
        private readonly StartScreen startScreen;
        private readonly MapScreen mapScreen;
        private readonly EndGameScreen endGameScreen;
        private readonly ContinueScreen continueScreen;

        public enum ScreenState
        {
            Start,
            Menu,
            Battling,
            Map,
            Continue,
            EndGame
        }

        private ScreenState screenState;

        public static readonly int Scale = Settings.Default.Scale;

        private MouseState _mouse;
        private MouseState _oldMouse;
        private KeyboardState _key;
        private KeyboardState _oldKey;

        static public SpriteFont endFont;
        static public SpriteFont mapFont;
        static public Texture2D pixel;

        public GlobalDrawArranger(DevimakuGame game)
        {
            mapFont = game.Content.Load<SpriteFont>("font/WizardFontFull2");
            endFont = game.Content.Load<SpriteFont>("font/PressStart2P");
            endFont.LineSpacing *= 2;

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            this.battleScreen = new BattleScreen(game);
            this.menuScreen = new MenuScreen(game);
            this.startScreen = new StartScreen(game);
            this.mapScreen = new MapScreen(game);
            this.endGameScreen = new EndGameScreen(game);
            this.continueScreen = new ContinueScreen();

            this.screenState = ScreenState.Menu;
        }

        /// <summary>
        /// Main draw cycle. Calls other drawers.
        /// </summary>
        public void CallDraws(DevimakuGame game, SpriteBatch defaultSpriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.SlateGray);
            defaultSpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            switch (screenState)
            {
                case ScreenState.Battling:
                    this.battleScreen.Draw(game, defaultSpriteBatch);
                    break;

                case ScreenState.Map:
                    graphicsDevice.Clear(Color.Black);
                    this.battleScreen.Draw(game, defaultSpriteBatch);
                    this.mapScreen.Draw(game, defaultSpriteBatch);
                    break;

                case ScreenState.EndGame:
                    this.battleScreen.Draw(game, defaultSpriteBatch);
                    endGameScreen.Draw(game, defaultSpriteBatch, battleScreen.IsPlayerDead);
                    break;

                case ScreenState.Start:
                    graphicsDevice.Clear(Color.Black);
                    this.startScreen.Draw(game, defaultSpriteBatch);
                    break;

                case ScreenState.Menu:
                    this.menuScreen.Draw(game, defaultSpriteBatch);
                    break;

                case ScreenState.Continue:
                    continueScreen.Draw(game, defaultSpriteBatch);
                    break;
            }

            //Draw pause overlay (temporal)
            if(Controls.isPauseToggled)
            {
                int scale = GlobalDrawArranger.Scale;
                int winX = DevimakuGame.defGameWidth * scale;
                int winY = DevimakuGame.defGameHeight * scale;
                var rect = new Rectangle(0, 0, winX, winY);

                const string msg = "Paused.\n\nPress P";
                var strPos = (rect.Size.ToVector2() - mapFont.MeasureString(msg)) / 2;

                defaultSpriteBatch.Draw(Shader.Fill(graphicsDevice, pixel, new Color(0, 0, 0, 170)), rect, Color.White);
                defaultSpriteBatch.DrawString(mapFont, msg, strPos, Color.White);
            }

            defaultSpriteBatch.End();
        }

        public void CallGuiControlUpdates(DevimakuGame game)
        {
            this._key = Keyboard.GetState();
            this._mouse = Mouse.GetState();

            //Toggling pause before anything
            if (Controls.CheckForSinglePress(_key, _oldKey, Controls.pause))
                Controls.ToggleControlPause();

            //No updates on pause
            if (Controls.isPauseToggled)
            {
                this._oldKey = this._key;
                this._oldMouse = this._mouse;

                return;
            }

            switch (screenState)
            {
                case ScreenState.Battling:
                    this.battleScreen.Update(game, this._mouse, _oldMouse, _key, _oldKey);
                    game.musicPlayer.InitStageLoopIfPossible(StageManager.CurrentStage);
                    if (!battleScreen.HasEnemies)
                    {
                        if (!(StageManager.CurrentStage >= StageManager.Stage.Center))
                        {
                            screenState = ScreenState.Map;
                            game.musicPlayer.InitOtherLoopIfPossible(screenState);
                            game.musicPlayer.PlaySound(Music.SoundEvent.StageClear);
                        }
                        else
                        {
                            screenState = ScreenState.EndGame;
                            game.musicPlayer.InitOtherLoopIfPossible(ScreenState.EndGame);
                        }
                    }

                    if (battleScreen.IsPlayerDead)
                    {
                        if (!continueScreen.HasAcceptedDefeat && !continueScreen.HasDecidedToFight)
                        {
                            screenState = ScreenState.Continue;
                            MediaPlayer.Stop();
                        }
                    }

                    break;

                case ScreenState.Map:
                    this.mapScreen.Update(game, this._mouse, _oldMouse, _key, _oldKey);
                    if (mapScreen.hasTriggeredAdvance)
                    {
                        screenState = ScreenState.Battling;
                        StageManager.NextStage(this.battleScreen);
                    }

                    break;

                case ScreenState.EndGame:
                    break;

                case ScreenState.Start:
                    this.startScreen.Update(game, this._mouse, _oldMouse, _key, _oldKey);
                    if (startScreen.hasPressedStart)
                        screenState = ScreenState.Battling;
                    break;

                case ScreenState.Menu:
                    this.menuScreen.Update(game, this._mouse, _oldMouse, _key, _oldKey);
                    if (menuScreen.hasTriggeredStartGame)
                        screenState = ScreenState.Start;
                    break;

                case ScreenState.Continue:
                    this.continueScreen.Update(game, this._mouse, _oldMouse, _key, _oldKey);

                    if (continueScreen.HasAcceptedDefeat)
                    {
                        screenState = ScreenState.EndGame;
                        MediaPlayer.Stop();
                    }
                    else if (continueScreen.HasDecidedToFight)
                    {
                        screenState = ScreenState.Battling;
                        battleScreen.RedeemPlayer();
                        continueScreen.Reset();
                        game.musicPlayer.InitStageLoopIfPossible(StageManager.CurrentStage, forceRestart: true);
                    }

                    break;
            }

            this._oldKey = this._key;
            this._oldMouse = this._mouse;
        }
    }
}
