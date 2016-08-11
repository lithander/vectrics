/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Text;

namespace Vectrics
{
    public struct Vector2D : IEquatable<Vector2D>, IComparable<Vector2D>
    {
        public static Vector2D Zero = new Vector2D(0, 0);
        public static Vector2D Half = new Vector2D(0.5f, 0.5f);
        public static Vector2D One = new Vector2D(1, 1);

        public static Vector2D Up = new Vector2D(0, -1.0f);
        public static Vector2D Left = new Vector2D(-1.0f, 0);
        public static Vector2D Right = new Vector2D(1.0f, 0);
        public static Vector2D Down = new Vector2D(0, 1.0f);

        public static Vector2D Void = new Vector2D(float.NaN, float.NaN);

        public bool IsVoid
        {
            get { return float.IsNaN(X) || float.IsNaN(Y); }
        }


        public float X;
        public float Y;

        //Constructors
        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Point2D p)
        {
            X = p.X;
            Y = p.Y;
        }

        public static Vector2D FromPolarRadian(float length, float angle)
        {
            return new Vector2D(length * (float)Math.Cos(angle), length * (float)Math.Sin(angle));
        }

        public static Vector2D FromInterpolation(Vector2D v1, Vector2D v2, float ratio)
        {
            return v1 + (v2 - v1) * ratio;
        }

        public Vector2D Sized(float newSize)
        {
            Vector2D result = this;
            result.Length = newSize;
            return result;
        }

        public Vector2D Clamped(float range)
        {
            if (Length > range)
                return this.Sized(range);
            return this;

        }

        public bool IsNonZero
        {
            get { return X != 0 || Y != 0; }
        }

        public bool IsZero
        {
            get { return X == 0 && Y == 0; }
        }

        private const float EPSILON_SQR = 0.000001f;
        public bool IsAlmostZero
        {
            get { return (X * X + Y * Y) < EPSILON_SQR; }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
            set
            {
                float len = (float)Math.Sqrt(X * X + Y * Y);
                if (len > 0)
                {
                    float f = value / len;
                    X *= f;
                    Y *= f;
                }
                else
                {
                    X = 0;
                    Y = 0;
                }
            }
        }

        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y;
            }
            set
            {
                float lenSqr = X * X + Y * Y;
                if (lenSqr > 0)
                {
                    float f = value / lenSqr;
                    X *= f;
                    Y *= f;
                }
                else
                {
                    X = 0;
                    Y = 0;
                }
            }
        }

        /**
         * The (signed) axis in which direction the vector points most
         * */
        public Vector2D DominantAxis
        {
            get
            {
                if (Math.Abs(X) > Math.Abs(Y))
                    return new Vector2D(X, 0);
                else
                    return new Vector2D(0, Y);
            }
        }

        
		/**
		 * An angle describing the orientation of the Vector. (-1,0) is 0. (1,0) is Pi. 
		 */
		public float PolarAngleRadian
		{
            get
            {
                return (float)(Math.Atan2(Y, X) + Math.PI);
            }

            set
            {
                float len = Length;
                X = len * (float)Math.Cos(value);
                Y = len * (float)Math.Sin(value);
            }
			/* 
			//the same effect but a lot slower..
			var r:Number = length;
			if(r == 0)
				return 0;
			else if(x >= 0)
				return Math.asin(y / r);
			else
				return Math.PI - Math.asin(y / r); 	
			*/
		}

        /**
		 * An angle describing the orientation of the Vector. (-1,0) is 0. (1,0) is 180. 
		 */
        public float PolarAngleDegree
        {
            get
            {
                return Angle.RadToDeg(PolarAngleRadian);
            }

            set
            {
                PolarAngleRadian = Angle.DegToRad(value);
            }
        }

        public void Reflect(Vector2D normal)
        {
            if(X == 0 && Y == 0)
                return;

            float dot = X * normal.X + Y * normal.Y;
            X -= 2 * normal.X * dot;
            Y -= 2 * normal.Y * dot;
        }

        public Vector2D Reflected(Vector2D normal)
        {
            Vector2D result = this;
            result.Reflect(normal);
            return result;
        }

        public void Reflect(Vector2D normal, float scale)
        {
            if (X == 0 && Y == 0)
                return;

            float dot = X * normal.X + Y * normal.Y;
            X -= (1 + scale) * normal.X * dot;
            Y -= (1 + scale) * normal.Y * dot;
        }

        public Vector2D Reflected(Vector2D normal, float scale)
        {
            Vector2D result = this;
            result.Reflect(normal, scale);
            return result;
        }

        public Vector2D ProjectedOn(Vector2D direction)
        {
            //Vector2D normalized = direction.Normalized();
            //return normalized * Dot(normalized);
            return direction * Dot(direction) / direction.LengthSquared;
        }

        //Operations
        /**
		 * Inverses this vector
		 */
		public void Invert()
		{
			X *= -1;
			Y *= -1;
		}

        /**
         * Returns a new vector of the same length but in the opposite direction.
         */
        public Vector2D Inverted()
        {
            return new Vector2D(-X,-Y);
        }
		
		/**
		 * Rotate this vector cw by 90°
		 */
        public void OrthogonalizeClockwise()
		{
			float tmp = X;
			X = -Y;
			Y = tmp;
		}

        /**
         * Rotate a copy of this vector that is orthogonal/perpendicular. (cw rotation by 90°).
         */
        public Vector2D OrthogonalizedClockwise()
        {
            return new Vector2D(-Y, X);
        }

        /**
         * Rotate this vector ccw by 90°
         */
        public void OrthogonalizeCounterClockwise()
        {
            float tmp = X;
            X = Y;
            Y = -tmp;
        }

        /**
         * Return a copy of this vector that is orthogonal/perpendicular. (ccw rotation by 90°).
         */
        public Vector2D OrthogonalizedCounterClockwise()
        {
            return new Vector2D(Y, -X);
        }
		
		/**
		 * Scales this vector to have unit length
		 */
		public void Normalize()
		{
            float len = Length;
            X /= len;
            Y /= len;
		}

        /**
         * Returns a new Vector of the same direction as this but with unit length.
         */
        public Vector2D Normalized()
        {
            float len = Length;
            return new Vector2D(X/len, Y/len);
        }

        public void SafeNormalize()
        {
            float s = Length;
            if (s != 0)
            {
                s = 1 / s;
                X *= s;
                Y *= s;
            }
            else
            {
                X = 0;
                Y = 0;
            }
        }

        public Vector2D SafeNormalized()
        {
            Vector2D result = this;
            result.SafeNormalize();
            return result;
        }
		
		/**
		 * Floors the components of the vector
		 */
		public void Floor()
		{
            X = (float)Math.Floor(X);
            Y = (float)Math.Floor(Y);
		}

        /**
        * Returns a new Vector matching this but with floord components.
        */
        public Vector2D Floored()
        {
            Vector2D result = this;
            result.Floor();
            return result;
        }
				
		/**
		 * Rounds the components of the vector to the nearest integer
		 */
		public void Round()
		{
            X = (float)Math.Round(X);
            Y = (float)Math.Round(Y);
		}
		
		/**
		 * Returns a new Vector matching this but with rounded components.
		 */
		public Vector2D Rounded()
		{
            Vector2D result = this;
            result.Round();
            return result;
		}		
        
        //Dot Product
        public float Dot(Vector2D v)
        {
            return X * v.X + Y * v.Y;
        }
        public static float Dot(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        //Cross (return magnitude of the 3D cross product parallel to Z)
        public float Cross(Vector2D v)
        {
            return X * v.Y - Y * v.X;
        }
        public static float Cross(Vector2D v1, Vector2D v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }

        /**
		 * Calculates the distance from the other vector.
		 */
		public float Distance(Vector2D v)
		{
            float dx = X - v.X;
            float dy = Y - v.Y;
            return (float)Math.Sqrt(dx*dx + dy*dy);
		}
        public static float Distance(Vector2D v1, Vector2D v2)
        {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
		
		/**
		 * Calculates the square of the distance from the other vector.
		 */
		public float DistanceSquared(Vector2D v)
		{
            float dx = X - v.X;
            float dy = Y - v.Y;
            return dx * dx + dy * dy;
		}
        public static float DistanceSquared(Vector2D v1, Vector2D v2)
        {
            float dx = v1.X - v2.X;
            float dy = v1.Y - v2.Y;
            return dx * dx + dy * dy;
        }

        //Rotate around Zero
        public void RotateDegree(float degree)
        {
            RotateRadian(Angle.DegToRad(degree));
        }
        public Vector2D RotatedDegree(float angle)
        {
            Vector2D result = this;
            result.RotateDegree(angle);
            return result;
        }
        public void RotateRadian(float radian)
        {
            /*
            float newAngle = Math.Atan2( Y, X ) + angle;
			float len = Length;
			X = len * Math.Cos( newAngle );
			Y = len * Math.Sin( newAngle );
            */
            //x' = x * cos ? - y * sin ? 
            float newx = (float)(X * Math.Cos(radian) - Y * Math.Sin(radian));
            //y' = x * sin ? + y * cos ?  
            Y = (float)(X * Math.Sin(radian) + Y * Math.Cos(radian));
            X = newx;
        }
        public Vector2D RotatedRadian(float angle)
        {
            Vector2D result = this;
            result.RotateRadian(angle);
            return result;
        }

        
        //Basic Arithmetic
		public static Vector2D operator-(Vector2D p)
		{
			return new Vector2D(-p.X, -p.Y);
		}
        public static Vector2D operator+(Vector2D p1, Vector2D p2)
        {
            return new Vector2D(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Vector2D operator-(Vector2D p1, Vector2D p2)
        {
            return new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Vector2D operator*(Vector2D p1, float scale)
        {
            return new Vector2D(p1.X * scale, p1.Y * scale);
        }
        public static Vector2D operator *(float scale, Vector2D p1)
        {
            return new Vector2D(p1.X * scale, p1.Y * scale);
        }
        public static Vector2D operator *(Vector2D p1, Vector2D p2)
        {
            return new Vector2D(p1.X * p2.X, p1.Y * p2.Y);
        }
        public static Vector2D operator/(Vector2D p1, float scale)
        {
            return new Vector2D(p1.X / scale, p1.Y / scale);
        }
        public static Vector2D operator /(float scale, Vector2D p1)
        {
            return new Vector2D(scale / p1.X, scale / p1.Y);
        }
        public static Vector2D operator /(Vector2D p1, Vector2D p2)
        {
            return new Vector2D(p1.X / p2.X, p1.Y / p2.Y);
        }
        public static bool operator==(Vector2D p1, Vector2D p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }
        public static bool operator!=(Vector2D p1, Vector2D p2)
        {
            return (p1.X != p2.X || p1.Y != p2.Y);
        }
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;
            Vector2D p = (Vector2D)obj;
            return (X == p.X && Y == p.Y);
        }

        public bool Equals(Vector2D p)
        {
            return (X == p.X && Y == p.Y);
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }
        
		public override string ToString()
		{
			return string.Format("({0} {1})", X, Y);
		}

        public int CompareTo(Vector2D other)
        {
            return LengthSquared.CompareTo(other.LengthSquared);
        }

        public static int CompareTo(Vector2D first, Vector2D second)
        {
            return first.LengthSquared.CompareTo(second.LengthSquared);
        }

        public static bool operator <(Vector2D x, Vector2D y) { return CompareTo(x, y) < 0; }
        public static bool operator >(Vector2D x, Vector2D y) { return CompareTo(x, y) > 0; }
        public static bool operator <=(Vector2D x, Vector2D y) { return CompareTo(x, y) <= 0; }
        public static bool operator >=(Vector2D x, Vector2D y) { return CompareTo(x, y) >= 0; }
    }
}
