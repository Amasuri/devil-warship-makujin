using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// <para>
    /// Will be stored as absolute references in some kind of List which would
    /// update every animation and also draw every non-dead thing at it's pos
    /// Inside BattleInterface i guess
    /// </para>
    /// <para>The invokator obviously inside things like, ugh, InputHandler or enemy move</para>
    /// <para>Yeah, those invokes will be under function with things like (index MAge) as args</para>
    /// </summary>
    public class BossAnimation
    {
        private readonly Texture2D _frameSheet; //one huge frame instead
        private readonly int _msBetweenFrames;
        private readonly int _maxFrameIndex;
        private int _currentFrame;
        private int _currentTime;
        private bool _isDead; //for things like spells which fade after
        private readonly List<Vector2> _activePositions;
        private Point _frameSize;

        public string Alias; //like, ice_spell

        public BossAnimation(DevimakuGame game, string spellNameInFS, int timeBetweenFrames = 80)
        {
            this._frameSheet = game.Content.Load<Texture2D>("res/spell/"+spellNameInFS);
            this._msBetweenFrames = timeBetweenFrames;
            this._maxFrameIndex = this._frameSheet.Width/this._frameSheet.Height;
            this._currentFrame = 0;
            this._currentTime = 0;
            this._isDead = true;
            this._frameSize = new Point(this._frameSheet.Height);
            this._activePositions = new List<Vector2>();

            this.Alias = spellNameInFS;
        }

        /// <summary>
        /// Call every cycle to update time between frames.
        /// </summary>
        public void UpdateFrame(GameTime gameTime)
        {
            if (this._isDead)
                return;

            this._currentTime += gameTime.ElapsedGameTime.Milliseconds;
            if (this._currentTime > this._msBetweenFrames)
            {
                this._currentTime -= this._msBetweenFrames;
                this._currentFrame++;
                if (this._currentFrame > this._maxFrameIndex)
                {
                    this._currentFrame = 0;
                    this._isDead = true;
                    this._activePositions.Clear();
                }
            }
        }

        /// <summary>
        /// Draws at current position(s!), if not dead.
        /// </summary>
        public void Draw(SpriteBatch spritebatch, int scale, SpriteEffects effect)
        {
            if(!this._isDead)
            {
                foreach (Vector2 activePosition in this._activePositions)
                {
                    spritebatch.Draw(this._frameSheet, activePosition, new Rectangle(
                            this._frameSize.X * this._currentFrame,
                            0,
                            this._frameSize.X,
                            this._frameSize.Y),
                        Color.White, 0f, Vector2.Zero, scale, effect, 1);
                }
            }
        }

        /// <summary>
        /// Makes not dead and remakes pos.
        /// Invoked by buttons, functions, etc
        /// Note that the position argument is added in a list, so you'd normally have like three invoke calls to make three of the same
        /// images draw at different positions.
        /// </summary>
        public void InvokeAtPos(Vector2 pos)
        {
            this._activePositions.Add(pos);
            this._isDead = false;
        }
    }
}
