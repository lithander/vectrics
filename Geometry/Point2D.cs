using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public struct Point2D
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Hash;

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
            Hash = X ^ (Y << 16);
        }

        //Conversions
        public static implicit operator Vector2D(Point2D p)
        {
            return new Vector2D(p.X, p.Y);
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (!(obj is Point2D))
                return false;
            
            // Return true if the fields match:
            return (X == ((Point2D)obj).X) && (Y == ((Point2D)obj).Y);
        }
                
        //Basic Arithmetic
        public static Vector2D operator -(Point2D p)
        {
            return new Vector2D(-p.X, -p.Y);
        }
        public static Vector2D operator +(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Vector2D operator *(Point2D p1, float scale)
        {
            return new Vector2D(p1.X * scale, p1.Y * scale);
        }
        public static Vector2D operator *(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X * p2.X, p1.Y * p2.Y);
        }
        public static Vector2D operator /(Point2D p1, float scale)
        {
            return new Vector2D(p1.X / scale, p1.Y / scale);
        }
        public static Vector2D operator /(Point2D p1, Point2D p2)
        {
            return new Vector2D(p1.X / p2.X, p1.Y / p2.Y);
        }
        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return (p1.X != p2.X || p1.Y != p2.Y);
        }
        public override int GetHashCode()
        {
            return Hash;
        }
    }
}
