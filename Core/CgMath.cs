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

        public static float Smerp(float from, float to, float t)
        {
            float weight = t * t * (3.0f - (2.0f * t));
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

        public static float Linstep(float edge0, float edge1, float v)
        {
            return Saturate((v - edge0) / (edge1 - edge0));
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


        public static Vector2D Max(Vector2D a, Vector2D b)
        {
            if (a.LengthSquared > b.LengthSquared)
                return a;
            else
                return b;
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
