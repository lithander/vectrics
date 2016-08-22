using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class CgMath
    {
        /*
         *      FLOAT
         */

        public static float Clamp(float x, float min, float max)
        {
            return (float)Math.Max(min, Math.Min(max, x));
        }

        public static float Frac(float v)
        {
            return (float)(v - Math.Floor(v));
        }

        public static float Saturate(float v)
        {
            return (float)Math.Max(0, Math.Min(1, v));
        }

        public static double Saturate(double v)
        {
            return Math.Max(0, Math.Min(1, v));
        }

        public static float SaturateSigned(float v)
        {
            return (float)Math.Max(-1, Math.Min(1, v));
        }

        public static double SaturateSigned(double v)
        {
            return Math.Max(-1, Math.Min(1, v));
        }

        public static float Lerp(float from, float to, float weight)
        {
            return from + weight * (to - from);
        }

        public static double Lerp(double from, double to, double weight)
        {
            return from + weight * (to - from);
        }

        public static float Smerp(float from, float to, float t)
        {
            float weight = t * t * (3.0f - (2.0f * t));
            return from + weight * (to - from);
        }

        public static double Smerp(double from, double to, double t)
        {
            double weight = t * t * (3.0 - (2.0 * t));
            return from + weight * (to - from);
        }

        public static float Towards(float from, float to, float max)
        {
            if (max <= 0)
                return from;
            return from + Math.Sign(to - from) * Math.Min(Math.Abs(to-from), max);
        }

        public static double Towards(double from, double to, double max)
        {
            if (max <= 0)
                return from;
            return from + Math.Sign(to - from) * Math.Min(Math.Abs(to - from), max);
        }
                
        public static float Step(float edge, float v)
        {
            return (v >= edge) ? 1 : 0; 
        }

        public static double Step(double edge, double v)
        {
            return (v >= edge) ? 1 : 0;
        }

        public static float Smoothstep(float edge0, float edge1, float v)
        {
            float t = Saturate((v - edge0) / (edge1 - edge0));
            return t * t * (3.0f - (2.0f * t));
        }

        public static double Smoothstep(double edge0, double edge1, double v)
        {
            double t = Saturate((v - edge0) / (edge1 - edge0));
            return t * t * (3.0 - (2.0 * t));
        }

        public static float Linstep(float edge0, float edge1, float v)
        {
            return Saturate((v - edge0) / (edge1 - edge0));
        }

        public static double Linstep(double edge0, double edge1, double v)
        {
            return Saturate((v - edge0) / (edge1 - edge0));
        }

        /*
        static float LinstepInOut(float edge0, float edge1, float edge2, float edge3, float v)

        constraint: 0 < edge0 < edge1 < edge2 < edge3
        accelerating in interval [edge0 edge1] from 0 to a constant speed in interval [edge1 edge2] then decelerating back to 0 in interval [edge2 edge3]
        returns values from 0 at t <= 0 to 1 at t >= t2       
           
        this is f(v)' and it shows the rate of growth of the return value over v

        f(v)'
        |
        |
        |h    ____________________edge2
        |    /edge1               \
        |   /                      \
        |__/________________________\______________ v
            edge0                     edge3

        h needs to be picked so that the integral over the function f(v)' which is f(v) = a + b + c
        is 0 for v < edge0 and sums up to 1 for v > edge3

        i = edge1 - edge0;
        l = edge2 - edge1;
        o = edge3 - edge2;

        t0 = i * Linstep(edge0, edge1, v);
        t1 = l * Linstep(edge1, edge2, v);
        t2 = o * Linstep(edge2, edge3, v);

        h = 1 / (0.5f * (i + o) + l);
        a = (0.5f * h / i) * t0 * t0;
        b = h * t1;
        c = t2 * h - (0.5f * h / o) * t2 * t2;
        
        RESULT:
        f(v) = a + b + c;
        */

        //constraint: 0 < edge0 < edge1 < edge2 < edge3
        public static float LinstepInOut(float edge0, float edge1, float edge2, float edge3, float v)
        {
            float i = edge1 - edge0;
            float l = edge2 - edge1;
            float o = edge3 - edge2;

            float t0 = (float)Math.Max(0, Math.Min(i, v - edge0));
            float t1 = (float)Math.Max(0, Math.Min(l, v - edge1));
            float t2 = (float)Math.Max(0, Math.Min(o, v - edge2));

            float r = i + o + 2 * l;
            float h = 2 / r;
            float result = t1 * h;
            if (t0 > 0)
                result += (t0 * t0) / (r * i);
            if (t2 > 0)
                result += (t2 * h) - (t2 * t2) / (r * o);
            return result;
        }

        //constraint: 0 < edge0 < edge1 < edge2 < edge3
        public static double LinstepInOut(double edge0, double edge1, double edge2, double edge3, double v)
        {
            double i = edge1 - edge0;
            double l = edge2 - edge1;
            double o = edge3 - edge2;

            double t0 = Math.Max(0, Math.Min(i, v - edge0));
            double t1 = Math.Max(0, Math.Min(l, v - edge1));
            double t2 = Math.Max(0, Math.Min(o, v - edge2));

            double r = i + o + 2 * l;
            double h = 2 / r;
            double result = t1 * h;
            if (t0 > 0)
                result += (t0 * t0) / (r * i);
            if (t2 > 0)
                result += (t2 * h) - (t2 * t2) / (r * o);
            return result;
        }

        public static float NextSmaller(float value)
        {
            int bits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            if (value > 0)
                return BitConverter.ToSingle(BitConverter.GetBytes(bits - 1), 0);
            else if (value < 0)
                return BitConverter.ToSingle(BitConverter.GetBytes(bits + 1), 0);
            else
                return -float.Epsilon;
        }

        public static float NextGreater(float value)
        {
            int bits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            if (value > 0)
                return BitConverter.ToSingle(BitConverter.GetBytes(bits + 1), 0);
            else if (value < 0)
                return BitConverter.ToSingle(BitConverter.GetBytes(bits - 1), 0);
            else
                return float.Epsilon;
        }

        /*
         *      VECTOR2D
         */

        public static Vector2D Abs(Vector2D v)
        {
            return new Vector2D(
               (float)Math.Abs(v.X),
               (float)Math.Abs(v.Y));
        }

        public static Vector2D Min(Vector2D a, Vector2D b)
        {
            return (a < b) ? a : b;
        }
        
        public static Vector2D Max(Vector2D a, Vector2D b)
        {
            return (a > b) ? a : b;
        }

        public static Vector2D Sign(Vector2D v)
        {
            return new Vector2D(
               (float)Math.Sign(v.X),
               (float)Math.Sign(v.Y));
        }

        public static Vector2D Round(Vector2D v)
        {
            return new Vector2D(
                (float)Math.Round(v.X),
                (float)Math.Round(v.Y));
        }

        public static Vector2D Floor(Vector2D v)
        {
            return new Vector2D(
                (float)Math.Floor(v.X),
                (float)Math.Floor(v.Y));
        }

        public static Vector2D Ceil(Vector2D v)
        {
            return new Vector2D(
                (float)Math.Ceiling(v.X),
                (float)Math.Ceiling(v.Y));
        }

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

        public static Vector2D LerpDirection(Vector2D from, Vector2D to, float weight)
        {
            float fromAngle = from.PolarAngleRadian;
            float toAngle = to.PolarAngleRadian;
            float delta = toAngle - fromAngle;
            delta = Angle.NormalizeRad(delta);
            return from.RotatedRadian(delta * weight);
        }

        public static Vector2D Smerp(Vector2D from, Vector2D to, float t)
        {
            float weight = t * t * (3.0f - (2.0f * t));
            return from + weight * (to - from);
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

        public static Vector2D Linstep(Vector2D edge0, Vector2D edge1, Vector2D v)
        {
            return new Vector2D(
                Linstep(edge0.X, edge1.X, v.X),
                Linstep(edge1.Y, edge1.Y, v.Y));
        }

        /*
        *      PARAMS
        */
        public static float Average(params float[] v)
        {
            return v.Average();
        }

        public static float Max(params float[] v)
        {
            return v.Max();
        }

        public static int Max(params int[] v)
        {
            return v.Max();
        }

        public static float Min(params float[] v)
        {
            return v.Min();
        }

        public static int Min(params int[] v)
        {
            return v.Min();
        }

        public static int Sign(params float[] v)
        {
            int sign = Math.Sign(v[0]);
            foreach (var value in v)
                if (Math.Sign(value) != sign)
                    return 0; //mixed or all zero

            return sign;
        }

        public static int Sign(params int[] v)
        {
            int sign = Math.Sign(v[0]);
            foreach (var value in v)
                if (Math.Sign(value) != sign)
                    return 0; //mixed or all zero

            return sign;
        }
        

        /*abs
        acos
        all
        any
        asin
        atan
        atan2
        bitCount
        bitfieldExtract
        bitfieldInsert
        bitfieldReverse
        ceil
        [clamp]
        clip
        cos
        cosh
        cross
        ddx
        ddy
        degrees
        determinant
        distance
        dot
        exp
        exp2
        faceforward
        findLSB
        findMSB
        floatToIntBits
        floatToRawIntBits
        floor
        fmod
        [frac]
        frexp
        fwidth
        intBitsToFloat
        inverse
        isfinite
        isinf
        isnan
        ldexp
        length
        lerp
        lit
        log
        log10
        log2
        max
        min
        modf
        mul
        normalize
        pack
        pow
        radians
        reflect
        refract
        round
        rsqrt
        [saturate]
        sign
        sin
        sincos
        sinh
        smoothstep
        sqrt
        step
        tan
        tanh
        tex1D
        tex1DARRAY
        tex1DARRAYbias
        tex1DARRAYcmpbias
        tex1DARRAYcmplod
        tex1DARRAYfetch
        tex1DARRAYlod
        tex1DARRAYproj
        tex1DARRAYsize
        tex1Dbias
        tex1Dcmpbias
        tex1Dcmplod
        tex1Dfetch
        tex1Dlod
        tex1Dproj
        tex1Dsize
        tex2D
        tex2Dbias
        tex2Dcmpbias
        tex2Dcmplod
        tex2Dfetch
        tex2Dlod
        tex2Dproj
        tex2Dsize
        tex2DARRAY
        tex2DARRAYbias
        tex2DARRAYfetch
        tex2DARRAYlod
        tex2DARRAYproj
        tex2DARRAYsize
        tex2DMSfetch
        tex2DMSsize
        tex2DMSARRAYfetch
        tex2DMSARRAYsize
        tex3D
        tex3Dbias
        tex3Dfetch
        tex3Dlod
        tex3Dproj
        tex3Dsize
        texBUF
        texBUFsize
        texCUBE
        texCUBEARRAY
        texCUBEARRAYbias
        texCUBEARRAYlod
        texCUBEARRAYsize
        texCUBEbias
        texCUBElod
        texCUBEproj
        texCUBEsize
        texRBUF
        texRBUFsize
        texRECT
        texRECTbias
        texRECTfetch
        texRECTlod
        texRECTproj
        texRECTsize
        transpose
        trunc
        unpack
         */
    }
}
