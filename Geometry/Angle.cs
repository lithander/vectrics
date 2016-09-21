/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public class Angle
    {
        public const float HalfPI = (float)(Math.PI * 0.5);
		public const float PI = (float)Math.PI;
        public const float TwoPI = (float)(Math.PI * 2);
        public const float Deg2Rad = (float)(Math.PI / 180);
        public const float Rad2Deg = (float)(180 / Math.PI);

        /*
		* Normalizes angle to be between -PI and +PI.
		*/
        public static float NormalizeRad(float angle)
		{
            return CgMath.Normalize(angle, -PI, PI);
		}		
		
		/*
		* Normalizes angle to be between -180 and +180.
		*/		
		public static float NormalizeDeg(float angle)
		{
            return CgMath.Normalize(angle, -180, 180);
		}
		
		/*
		* Normalizes angle to be between 0 and 2PI.
		*/		
		public static float NormalizeRad2(float angle)
		{
            return CgMath.Normalize(angle, 0, TwoPI);
		}		
		
		/*
		* Normalizes angle to be between 0 and +360.
		*/		
		public static float NormalizeDeg2(float angle)
		{
            return CgMath.Normalize(angle, 0, 360);
		}
		
		/**
		 * Is an angle in the cone defined by min & max?
		 * Assumes angles are normalized to be between -PI and +PI.
		 */		
		public static bool IsEnclosedRad(float angle, float min, float max)
		{
			angle += PI;
			min += PI;
			max += PI;
			return (angle > min && angle < max);
		}
		
		/**
		 * These companion methods will convert radians to degrees
		 * and degrees to radians.
		 */		
		public static float RadToDeg(float radians)
		{
			return radians * Rad2Deg;
		}
		
		public static float DegToRad(float degrees)
		{
			return degrees * Deg2Rad;
		}	
		
        //v & v2 need to be normalized
		public static float ConeAngleRadian(Vector2D v, Vector2D v2)
		{
            if (v == v2 || v.IsZero || v2.IsZero)
                return 0.0f;

            return NormalizeRad((float)Math.Atan2(v.Y, v.X) - (float)Math.Atan2(v2.Y, v2.X));
		}

        //v & v2 need to be normalized
        public static float ConeAngleDegree(Vector2D v, Vector2D v2)
        {
            if (v == v2)
                return 0.0f;

            return NormalizeRad((float)Math.Atan2(v.Y, v.X) - (float)Math.Atan2(v2.Y, v2.X)) * Rad2Deg;
        }

        //v & v2 need to be normalized
		public static float PolarAngleRadian(Vector2D v)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}

        //v & v2 need to be normalized
        public static float PolarAngleDegree(Vector2D v)
        {
            return (float)Math.Atan2(v.Y, v.X) * Rad2Deg;
        }
    }
}
