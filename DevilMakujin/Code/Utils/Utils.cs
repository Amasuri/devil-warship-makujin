using Microsoft.Xna.Framework;
using System;

namespace DevilMakujin.Code
{
    static public class Utils
    {
        public static float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        static public float AngleFromSpeed(Vector2 speed, float maxSpeed)
        {
            Vector2 mainDirection = new Vector2(0, -1);
            Vector2 normalSpeed = new Vector2(Utils.Remap(speed.X, -maxSpeed, maxSpeed, -1, 1), Utils.Remap(speed.Y, -maxSpeed, maxSpeed, -1, 1));
            float radian = (float)(Math.Atan2(normalSpeed.Y, normalSpeed.X) - Math.Atan2(mainDirection.Y, mainDirection.X));
            return radian;
        }
    }

    public struct Circle
    {
        public Vector2 Center { get; }
        public float Radius { get; }

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Contains(Vector2 point)
        {
            return (point - Center).Length() <= Radius;
        }

        public float GetDistanceFromCenterInRadii(Vector2 point)
        {
            return (point - Center).Length() / Radius;
        }
    }
}
