using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public static class Rnd
    {
        private static Random _rnd = new Random();

        public static Vector2D V2D(float minLength, float maxLength)
        {
            float angle = Angle.TwoPI * (float)_rnd.NextDouble();
            float length = minLength + maxLength * (float)_rnd.NextDouble();
            return Vector2D.FromPolarRadian(length, angle);
        }

        public static T Draw<T>(IEnumerable<T> from)
        {
            int cnt = from.Count();
            if (cnt <= 0)
                return default(T);

            int idx = _rnd.Next(0, cnt);//upper bound exclusive
            return from.ElementAt(idx);
        }
    }
}
