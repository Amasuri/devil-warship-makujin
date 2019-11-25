using DevilMakujin.Code;
using DevilMakujin.Code.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DevilMakujin
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DevimakuGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _defaultSpriteBatch;

        /// <summary>
        /// Do not make this public, since it's an arranger which contains every other class. It's not
        /// supposed to be see-able; the game class is what you need for common info.
        /// </summary>
        private GlobalDrawArranger _drawArr;

        public Code.Music.MusicPlayer musicPlayer;
        public GameTime refGameTime;

        static public TimeSpan delta;
        static public float DeltaUpdate => delta.Milliseconds;

        public const int defGameWidth = 256 * 2;
        public const int defGameHeight = 160 * 2;

        static public Random Rand = new Random();

        public string Lang { get; private set; } //for later localization purposes

        public DevimakuGame()
        {
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content/res";

            this._graphics.PreferredBackBufferWidth = defGameWidth * GlobalDrawArranger.Scale;
            this._graphics.PreferredBackBufferHeight = defGameHeight * GlobalDrawArranger.Scale;

            this.refGameTime = new GameTime();

            delta = new TimeSpan();

            this.Lang = "eng";
        }

        override protected void Initialize()
        {
            AEntity.InitSharedAssets(this);
            EnemySet.InitEnemyListings(this);

            base.Initialize();
        }

        override protected void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this._defaultSpriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.musicPlayer = new Code.Music.MusicPlayer(this);
            this._drawArr = new GlobalDrawArranger(this);
        }

        override protected void UnloadContent()
        {
        }

        override protected void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            refGameTime = gameTime;
            delta = gameTime.ElapsedGameTime;

            this._drawArr.CallGuiControlUpdates(this);

            base.Update(gameTime);
        }

        override protected void Draw(GameTime gameTime)
        {
            this._drawArr.CallDraws(this, this._defaultSpriteBatch, this.GraphicsDevice);

            base.Draw(gameTime);
        }

        public void ChangeLang()
        {
            //Could be cooler to raise a "lang change" event here and do all the jibs automatically
            if (this.Lang == "eng")
                this.Lang = "ru";
            else
                this.Lang = "eng";
        }
    }
}
