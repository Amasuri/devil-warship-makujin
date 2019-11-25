using DevilMakujin.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DevilMakujin.Code.Entity
{
    static public class PlayerPhysics
    {
        public static Vector2 playerAbsPos { get; private set; }

        /// <summary>
        /// <para>Non-asteroids "speed" variable, which is basically X and Y position difference.</para>
        /// <para>Non-asteroids control schemes work on this one directly.</para>
        /// <para>Asteroids control scheme uses speed and angle variables to set this anew.</para>
        /// </summary>
        public static Vector2 playerPosDiff { get; private set; }

        /// <summary>
        /// Asteroids speed variable, which is a non-negative value. It's purpose is to form a diff Vector based on
        /// both this and rotation variable.
        /// </summary>
        public static float astPlayerSpeed { get; private set; }

        //These two are different: last thrust rotation versus free spin rotation
        public static float astFacingPlayerRotation { get; private set; }

        private static float astDirectionPlayerRotation;

        #region Physics constants for each control mode

        private static readonly float oldSpeedDescrease = Settings.Default.OldPhysicsSpeedDescreaseMultiplier; //0.95f; //0.95f
        private static readonly float oldAcceleration = Settings.Default.OldPhysicsAcceleration;//0.5f;
        private static readonly float oldMaxSpeed = Settings.Default.OldPhysicsMaxEntitySpeed;//20;
        private static readonly float oldMinimalAbsSpeed = Settings.Default.OldPhysicsMinSpeedThreshold;//0.1f;

        private static readonly float astSpeedDescrease = Settings.Default.AstPhysicsSpeedDescrease;//0.99f;
        private static readonly float astAcceleration = Settings.Default.AstPhysicsAcceleration;//0.5f;
        private static readonly float astMaxSpeed = Settings.Default.AstPhysicsMaxSpeed;//oldMaxSpeed;
        private static readonly float astMinimalAbsSpeed = Settings.Default.AstPhysicsMinSpeed;//0.05f;
        private static readonly float astRotationRate = Settings.Default.AstPhysicsRotationRate;//0.1f;

        #endregion Physics constants for each control mode

        static public void InitPosOnStage(StageManager.Stage stage, Vector2 playerDrawOffset)
        {
            if (stage == StageManager.Stage.Center)
                playerAbsPos = new Vector2(0, 100 * 5);
            else
                playerAbsPos = Vector2.Zero;

            playerAbsPos -= playerDrawOffset;
            playerPosDiff = Vector2.Zero;

            astFacingPlayerRotation = 0.0f;
            astDirectionPlayerRotation = 0.0f;
            astPlayerSpeed = 0.0f;
        }

        static public void ChangePlayerSpeedBasedOnMode(KeyboardState keys, KeyboardState oldKeys, Controls.ShipMode controlMode)
        {
            if (controlMode == Controls.ShipMode.Old)
            {
                OldPhysicsScheme(keys);
            }
            else if (controlMode == Controls.ShipMode.Asteroids)
            {
                AsteroidsPhysicsScheme(keys);
            }
            else if (controlMode == Controls.ShipMode.Sinistar)
            {
                SinistarPhysicsScheme(keys);
            }

            UpdatePos(controlMode);
        }

        private static void SinistarPhysicsScheme(KeyboardState keys)
        {
            //Sinistar input is only 8 possible directions, but there's a gradual rotation between old and new direction
            var newHeadingVector = new Vector2();
            if (keys.IsKeyDown(Controls.up))
                newHeadingVector = new Vector2(newHeadingVector.X, -1);
            else if (keys.IsKeyDown(Controls.down))
                newHeadingVector = new Vector2(newHeadingVector.X, 1);
            if (keys.IsKeyDown(Controls.left))
                newHeadingVector = new Vector2(-1, newHeadingVector.Y);
            else if (keys.IsKeyDown(Controls.right))
                newHeadingVector = new Vector2(1, newHeadingVector.Y);
            newHeadingVector.Normalize();

            //(Huge WIP as i figure out how to make it, nvm the obvious mistakes)
        }

        private static void AsteroidsPhysicsScheme(KeyboardState keys)
        {
            //Descrease speed over time (lose inertia)
            astPlayerSpeed *= astSpeedDescrease;

            //Change rotation and thrust based on arrows
            if (keys.IsKeyDown(Controls.left))
                astFacingPlayerRotation -= astRotationRate;
            else if (keys.IsKeyDown(Controls.right))
                astFacingPlayerRotation += astRotationRate;
            if (keys.IsKeyDown(Controls.up) || keys.IsKeyDown(Controls.down))
            {
                //Thrust or dethrust
                if (keys.IsKeyDown(Controls.up))
                    astPlayerSpeed += astAcceleration;
                else if (keys.IsKeyDown(Controls.down))
                    astPlayerSpeed -= astAcceleration;

                //Update used-in-move-diff rotation
                astDirectionPlayerRotation = astFacingPlayerRotation;
            }

            //Cut excess speed
            if (astPlayerSpeed > astMaxSpeed)
                astPlayerSpeed = astMaxSpeed;
            if (astPlayerSpeed < astMinimalAbsSpeed)
                astPlayerSpeed = 0;

            //Make a rotation -1..1 Vector based on rotation and multiply it by speed
            Vector2 headingVector = new Vector2
            (
                (float)Math.Cos(astDirectionPlayerRotation),
                (float)Math.Sin(astDirectionPlayerRotation)
            );
            headingVector.Normalize();

            playerPosDiff = headingVector * astPlayerSpeed;

            //TODO: add some kind of proximization function between old poss diff vector and new vector, which would gradually change the old one into new
            //while the old one is used. (btw, rotations don't work, so implement something based on old/new pos diff gradual flow)
        }

        private static void OldPhysicsScheme(KeyboardState keys)
        {
            //Make the moves a bit more smooth and fluid
            playerPosDiff *= oldSpeedDescrease;
            if (Math.Abs(playerPosDiff.X) <= oldMinimalAbsSpeed)
                playerPosDiff = new Vector2(0, playerPosDiff.Y);
            if (Math.Abs(playerPosDiff.Y) <= oldMinimalAbsSpeed)
                playerPosDiff = new Vector2(playerPosDiff.X, 0);

            //Check for input
            float acceleration = oldAcceleration;
            float maxSpeed = oldMaxSpeed;

            if (keys.IsKeyDown(Controls.left))
                playerPosDiff = new Vector2(playerPosDiff.X - acceleration, playerPosDiff.Y);
            else if (keys.IsKeyDown(Controls.right))
                playerPosDiff = new Vector2(playerPosDiff.X + acceleration, playerPosDiff.Y);
            if (keys.IsKeyDown(Controls.up))
                playerPosDiff = new Vector2(playerPosDiff.X, playerPosDiff.Y - acceleration);
            else if (keys.IsKeyDown(Controls.down))
                playerPosDiff = new Vector2(playerPosDiff.X, playerPosDiff.Y + acceleration);

            //Cut out accelerated vector above the maxSpeed maximum
            float speedRatioX = Math.Abs(playerPosDiff.X / maxSpeed);
            float speedRatioY = Math.Abs(playerPosDiff.Y / maxSpeed);
            if (speedRatioX >= 1.0f)
                playerPosDiff = new Vector2(playerPosDiff.X / speedRatioX, playerPosDiff.Y);
            if (speedRatioY >= 1.0f)
                playerPosDiff = new Vector2(playerPosDiff.X, playerPosDiff.Y / speedRatioY);
        }

        static public float GetMaxPossibleSpeed(Controls.ShipMode mode)
        {
            if (mode == Controls.ShipMode.Old)
                return oldMaxSpeed;
            else if (mode == Controls.ShipMode.Asteroids)
                return astMaxSpeed;
            else
                throw new NotImplementedException();
        }

        private static void UpdatePos(Controls.ShipMode mode)
        {
            playerAbsPos += playerPosDiff;
        }

        internal static void RevertSpeed()
        {
            playerPosDiff = -playerPosDiff;
        }

        public static float GetRotationForDraw(Controls.ShipMode mode)
        {
            if (mode == Controls.ShipMode.Asteroids)
                return (float)(astFacingPlayerRotation + (Math.PI / 2));
            else
                return Utils.AngleFromSpeed(playerPosDiff, oldMaxSpeed);
        }

        /// <summary>
        /// In old and asteroids control schemes, shooting is drastically different
        /// </summary>
        public static Vector2 GetShootingDirection(Controls.ShipMode mode)
        {
            if (mode == Controls.ShipMode.Old)
            {
                var maxSpeed = GetMaxPossibleSpeed(Controls.mode);
                Vector2 shotDirection = new Vector2
                (
                    Utils.Remap(playerPosDiff.X, -maxSpeed, maxSpeed, -1, 1),
                    Utils.Remap(playerPosDiff.Y, -maxSpeed, maxSpeed, -1, 1)
                );

                shotDirection.Normalize();

                return shotDirection;
            }
            else if (mode == Controls.ShipMode.Asteroids)
            {
                Vector2 shotDiretion = new Vector2
                (
                    (float)Math.Cos(astFacingPlayerRotation),
                    (float)Math.Sin(astFacingPlayerRotation)
                );

                return shotDiretion;
            }
            else
            {
                throw new NotImplementedException("No sinistar controls");
            }
        }
    }
}
