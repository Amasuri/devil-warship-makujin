using DevilMakujin.Properties;
using Microsoft.Xna.Framework.Input;
using System;

namespace DevilMakujin.Code
{
    static public class Controls
    {
        public static ShipMode mode = (ShipMode)Settings.Default.DefaultPhysicsMode;

        public enum ShipMode
        {
            Old,
            Asteroids,
            Sinistar
        }

        public static Keys up = Keys.Up;
        public static Keys down = Keys.Down;
        public static Keys left = Keys.Left;
        public static Keys right = Keys.Right;

        public static Keys shoot = Keys.Z;

        public static Keys mute = Keys.S;
        public static Keys accept = shoot;
        public static Keys cancel = Keys.X;

        public static Keys pause = Keys.P;

        public static Keys debugWeaponSwitch = cancel;
        public static Keys debugControlSwitch = Keys.C;

        private const Keys invi = Keys.M;
        private const Keys sib = Keys.A;
        private const Keys ili = Keys.K;
        private const Keys ty = Keys.U;

        static public bool isPauseToggled { get; private set; }

        static public bool CheckForSinglePress(KeyboardState keyboard, KeyboardState oldKeyboard, Keys checkKey)
        {
            return keyboard.IsKeyDown(checkKey) && oldKeyboard.IsKeyUp(checkKey);
        }

        static public bool CheckForInvisibilityCheat(KeyboardState keyboard)
        {
            return keyboard.IsKeyDown(invi) && keyboard.IsKeyDown(sib) && keyboard.IsKeyDown(ili) && keyboard.IsKeyDown(ty);
        }

        /// <summary>
        /// Automatically looks if criteria were met and changes accordingly
        /// </summary>
        static public void DebugChangeControlScheme(KeyboardState keys, KeyboardState oldKeys)
        {
            if (!CheckForSinglePress(keys, oldKeys, debugControlSwitch))
                return;

            mode++;
            if (mode == ShipMode.Sinistar)
                mode = ShipMode.Old;
        }

        static public void ToggleControlPause()
        {
            isPauseToggled = !isPauseToggled;
        }

        static public bool CheckForClick(MouseState mouse, MouseState oldMouse)
        {
            return mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
        }
    }
}
