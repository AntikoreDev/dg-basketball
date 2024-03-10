using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.BBMod
{
    static class Utils
    {
        public static Vec2 Rotate(Vec2 vector, Vec2 around, float radians)
        {
            Vec2 position = vector - around;
            position = position.Multiply(RotationMatrix(radians));
            return around + position;
        }

        public static Vec2 Multiply(this Vec2 vector, float[,] m)
        {
            return new Vec2(vector.x * m[0, 0] + vector.y * m[1, 0], vector.x * m[0, 1] + vector.y * m[1, 1]);
        }

        public static float[,] RotationMatrix(float radians)
        {
            return new float[2, 2]
            {
                {
                    (float)Math.Cos(radians),
                    (float)Math.Sin(radians)
                },
                {
                    -(float)Math.Sin(radians),
                    (float)Math.Cos(radians)
                }
            };
        }
    }
}
