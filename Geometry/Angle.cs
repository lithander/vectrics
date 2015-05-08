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
		
		/*
		* Normalizes angle to be between -PI and +PI.
		*/		
		public static float NormalizeRad(float angle)
		{
			while( angle < -PI)
				angle += TwoPI;
			while( angle > PI)
				angle -= TwoPI;
			return angle;
		}		
		
		/*
		* Normalizes angle to be between -180 and +180.
		*/		
		public static float NormalizeDeg(float angle)
		{
			while( angle < -180)
				angle += 360;
			while( angle > 180)
				angle -= 360;
			return angle;
		}
		
		/*
		* Normalizes angle to be between 0 and 2PI.
		*/		
		public static float NormalizeRad2(float angle)
		{
			while( angle < 0)
				angle += TwoPI;
			while( angle > TwoPI)
				angle -= TwoPI;
			return angle;
		}		
		
		/*
		* Normalizes angle to be between 0 and +360.
		*/		
		public static float NormalizeDeg2(float angle)
		{
			while( angle < 0)
				angle += 360;
			while( angle > 360)
				angle -= 360;
			return angle;
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
			return radians * 180 / PI;
		}
		
		public static float DegToRad(float degrees)
		{
			return degrees * PI / 180;
		}	
		
		public static float ConeAngleRadian(Vector2D v, Vector2D v2)
		{
            if (v == v2)
                return 0.0f;

            return NormalizeRad((float)Math.Atan2(v.Y, v.X) - (float)Math.Atan2(v2.Y, v2.X));
		}
		
		public static float PolarAngleRadian(Vector2D v)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}
    }
}
