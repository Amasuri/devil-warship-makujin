using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DevilMakujin.Code.Graphics
{
    /// <summary>
    /// A special class that loads several images and on .Draw() cycle iterates over them per some time.
    /// </summary>
    public class Animation
    {
        protected int changeFrameAfterThisMs;
        protected int currentFrameIndex;
        protected int maxFrameIndex;
        protected double currentTimeMs;
        protected Point frameSize;

        protected bool isDrawn;
        protected float imageScale;
        protected bool isALoop;
        private Vector2 usualPos;

        private readonly Texture2D img;
        private readonly Vector2 spriteSheetOffset;
        private readonly int margin;

        /// <summary>
        /// Loads an img strip on path (justified in one-stripe-per-anim scenario) using specified metadata.
        /// </summary>
        public Animation(Game game, string path, Point frameSize, int maxFrames, float imgScale = 1.0f, int msBetweenFrames = 80, int margin = 0)
        {
            this.img = game.Content.Load<Texture2D>(path);

            this.changeFrameAfterThisMs = msBetweenFrames;
            this.currentFrameIndex = 0;
            this.currentTimeMs = 0;
            this.isDrawn = false;

            this.maxFrameIndex = maxFrames - 1;
            this.frameSize = frameSize;

            this.imageScale = imgScale;
            this.isALoop = false;

            this.spriteSheetOffset = Vector2.Zero;

            this.margin = margin;
        }

        /// <summary>
        /// Uses an existing image (justified in huge character all-anim sets) to set metadata to later use that image directly for drawing
        /// </summary>
        public Animation(Texture2D sprSheet, Point frameSize, int maxFrames, float imgScale = 1.0f, int msBetweenFrames = 80, Vector2 startPosOnSheet = default, int margin = 0)
        {
            this.img = sprSheet;

            this.changeFrameAfterThisMs = msBetweenFrames;
            this.currentFrameIndex = 0;
            this.currentTimeMs = 0;
            this.isDrawn = false;

            this.maxFrameIndex = maxFrames - 1;
            this.frameSize = frameSize;

            this.imageScale = imgScale;
            this.isALoop = false;

            this.spriteSheetOffset = startPosOnSheet != null ? startPosOnSheet : Vector2.Zero;

            this.margin = margin;
        }

        /// <summary>
        /// <para>Draws current frame of animation at pos.</para>
        /// <para>If "Draw on pos" is specified on enabling, then draws this pos + the enabled pos.</para>
        /// </summary>
        public void Draw(SpriteBatch spritebatch, SpriteEffects effects, double delta, Vector2 pos, bool doTick = true, float scale = 1)
        {
            if (!this.isDrawn)
                return;

            spritebatch.Draw(this.img, this.usualPos + pos, new Rectangle(
                    ((frameSize.X + margin) * currentFrameIndex) + (int)spriteSheetOffset.X,
                    0 + (int)spriteSheetOffset.Y,
                    frameSize.X,
                    frameSize.Y),
                Color.White, 0f, Vector2.Zero, scale, effects, 1);

            if (doTick)
                this.Tick(delta);
        }

        /// <summary>
        /// At set ms intervals, rotate current frame indexes whithin [0 , maxIndex].
        /// Usually is called inside draw, but can be called separatedly for shared assets
        /// (to not Tick gajillion times for one obj and multiple draws)
        /// </summary>
        public void Tick(double delta)
        {
            this.currentTimeMs += delta;

            //Inscrementing frame, if the gap is more that standart set gap
            if (this.currentTimeMs >= this.changeFrameAfterThisMs)
            {
                this.currentFrameIndex++;
                this.currentTimeMs -= this.changeFrameAfterThisMs;
            }

            //On last frame, either start over or stop drawing, depending on how was animation initialized
            if (this.currentFrameIndex > this.maxFrameIndex)
            {
                if (this.isALoop)
                    this.currentFrameIndex = 0;
                else
                    this.DisableDrawing();
            }
        }

        /// <summary>
        /// Do actions necessary on enabling (resetting index, deciding loopable/one-shot), but only if not already set.
        /// </summary>
        public void EnableDrawing(bool isALoop = true)
        {
            if (this.isDrawn)
                return;

            this.isDrawn = true;
            this.isALoop = isALoop;
            this.currentFrameIndex = 0;
        }

        /// <summary>
        /// Do actions necessary on enabling (resetting index, deciding loopable/one-shot), but only if not already set.
        /// Also changes draw pos.
        /// </summary>
        public void EnableDrawingOnPos(Vector2 pos, bool isALoop = true)
        {
            if (pos != null)
                this.usualPos = pos;
            else
                this.usualPos = Vector2.Zero;

            EnableDrawing(isALoop);
        }

        /// <summary>
        /// Do actions necessary on disabling (resetting index), but only if not already disabled.
        /// </summary>
        public void DisableDrawing()
        {
            if (!this.isDrawn)
                return;

            this.isDrawn = false;
            this.currentFrameIndex = 0;
        }

        public Vector2 GetSingleFrameSize()
        {
            return frameSize.ToVector2();
        }
    }
}
