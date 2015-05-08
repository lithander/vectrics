using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Vectrics
{
    public class CgMath
    {
        /*
         *      FLOAT
         */

        public static float Clamp(float x, float a, float b)
        {
            return (float)Math.Max(a, Math.Min(b, x));
        }

        public static float Frac(float v)
        {
            return (float)(v - Math.Floor(v));
        }

        public static float Saturate(float v)
        {
            return (float)Math.Max(0, Math.Min(1, v));
        }

        public static float SaturateSigned(float v)
        {
            return (float)Math.Max(-1, Math.Min(1, v));
        }

        public static float Lerp(float from, float to, float weight)
        {
            return from + weight * (to - from);
        }

        public static float Towards(float from, float to, float max)
        {
            if (max <= 0)
                return from;
            return from + Math.Sign(to - from) * Math.Min(Math.Abs(to-from), max);
        }

        public static float Step(float edge, float v)
        {
            return (v >= edge) ? 1 : 0; 
        }        

        public static float Smoothstep(float edge0, float edge1, float v)
        {
            float t = Saturate((v - edge0) / (edge1 - edge0));
            return t * t * (3.0f - (2.0f * t));
        }

        /*
         *      VECTOR2D
         */

        public static Vector2D Clamp(Vector2D x, Vector2D a, Vector2D b)
        {
            return new Vector2D(
                Clamp(x.X, a.X, b.X),
                Clamp(x.Y, a.Y, b.Y));
        }

        public static Vector2D Frac(Vector2D v)
        {
            return new Vector2D(
                Frac(v.X),
                Frac(v.Y));
        }

        public static Vector2D Saturate(Vector2D v)
        {
            return new Vector2D(
                Saturate(v.X),
                Saturate(v.Y));
        }

        public static Vector2D SaturateSigned(Vector2D v)
        {
            return new Vector2D(
                SaturateSigned(v.X),
                SaturateSigned(v.Y));
        }
        
        public static Vector2D Towards(Vector2D from, Vector2D to, float maxDistance)
        {
            if (maxDistance <= 0)
                return from;
            float w = (to - from).Length / maxDistance;
            return from + (to - from) / Math.Max(1, w);
        }

        public static Vector2D Lerp(Vector2D from, Vector2D to, float weight)
        {
            return from + (to - from) * weight;
        }

        public static Vector2D Step(Vector2D edge, Vector2D v)
        {
            return new Vector2D(
                Step(edge.X, v.X),
                Step(edge.Y, v.Y));
        }

        public static Vector2D Smoothstep(Vector2D edge0, Vector2D edge1, Vector2D v)
        {
            return new Vector2D(
                Smoothstep(edge0.X, edge1.X, v.X),
                Smoothstep(edge1.Y, edge1.Y, v.Y));
        }

    }
}
