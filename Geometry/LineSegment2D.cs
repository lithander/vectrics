/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public class LineSegment2D
    {
        [Flags]
        public enum Relation
        {
            NoConnection = 0x0,
            StartConnects = 0x1,
            EndConnects = 0x2,
            OtherStartConnects = 0x4,
            OtherEndConnects = 0x8,
            Intersection = 0xF
        };

        public Vector2D Start;
        public Vector2D End;

        /**
         * A new vector covering the distance from start to end.
         */
        public Vector2D StartToEnd
        {
            get
            {
                return End - Start;
            }

            set
            {
                End.Set(Start + value);
            }
        }

        /**
         * A new vector covering the distance from start to end.
         */
        public Vector2D EndToStart
        {
            get
            {
                return Start - End;
            }

            set
            {
                Start.Set(End - value);
            }
        }

        public LineSegment2D(Vector2D startPoint, Vector2D endPoint)
        {
            Start = startPoint;
            End = endPoint;
        }

        public LineSegment2D(int x, int y, int x2, int y2)
        {
            Start.Set(x, y);
            End.Set(x2, y2);
        }

        public void SetRef(Vector2D startPoint, Vector2D endPoint)
        {
            Start = startPoint;
            End = endPoint;
        }

        public void SetCopy(Vector2D startPoint, Vector2D endPoint)
        {
            Start.Set(startPoint);
            End.Set(endPoint);
        }

        public float Length
        {
            get
            {
                return StartToEnd.Length;
            }
        }

        public float LengthSquared
        {
            get
            {
                return StartToEnd.LengthSquared;
            }
        }

        /**
		 * Make a copy of this line segment.
		 */
        public LineSegment2D Clone()
        {
            return new LineSegment2D(Start, End);
        }

        public static bool operator ==(LineSegment2D a, LineSegment2D b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
                return false;

            return (a.Start == b.Start && a.End == b.End);
        }

        public static bool operator !=(LineSegment2D a, LineSegment2D b)
        {
            // If both are null, or both are same instance, return false.
            if (Object.ReferenceEquals(a, b))
                return false;

            // If one is null, but not both, return true.
            if (((object)a == null) || ((object)b == null))
                return true;

            return (a.Start != b.Start || a.End == b.End);
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;
            LineSegment2D l = (LineSegment2D)obj;
            return (Start == l.Start && End == l.End);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        /**
         * Compare this line segment to another and return true if the lines are parallel.
         */
        public bool IsParallel(LineSegment2D other, float tolerance = 0)
        {
            return Math.Abs(StartToEnd.Cross(other.StartToEnd)) <= tolerance;
        }

        public bool IsRightSide(Vector2D pt)
        {
            //its the basic idea of the crossproduct in R3 with a garuanteed 0 for z.
            return Start.X * (End.Y - pt.Y) + End.X * (pt.Y - Start.Y) + pt.X * (Start.Y - End.Y) > 0;
        }

        public bool IsSameSide(Vector2D a, Vector2D b)
        {
            //its the basic idea of the crossproduct in R3 with a garuanteed 0 for z.
            float aSide = Start.X * (End.Y - a.Y) + End.X * (a.Y - Start.Y) + a.X * (Start.Y - End.Y);
            float bSide = Start.X * (End.Y - b.Y) + End.X * (b.Y - Start.Y) + b.X * (Start.Y - End.Y);
            return aSide * bSide > 0; //either both positive or both negative
        }

        /**
         * Compare this line segment to another and return true if the lines have an intersection.
         */
        /*
        public bool IsIntersecting(LineSegment2D other)
        {
            //line_this = start + a * direction
            //line_other = other.start + b * direction
            //start.x + a * direction.x = other.start.x + b * other.direction.x;
            //start.y + a * direction.y = other.start.y + b * other.direction.y;
            //solve to a...	

            Vector2D v = Vector;
            Vector2D ov = other.Vector;
            Vector2D s = Start - other.Start;

            float denom = v.Cross(ov);
            float a = ov.Cross(s) / denom;
            float b = v.Cross(s) / denom;

            //The equations apply to lines, if the intersection of line segments is required then it is only necessary to test if ua and ub lie between 0 and 1. 
            //Whichever one lies within that range then the corresponding line segment contains the intersection point. 
            //If both lie within the range of 0 to 1 then the intersection point is within both line segments. 
            return (a > 0 && a < 1 && b > 0 && b < 1);
        }
        */

        public bool IsIntersecting(LineSegment2D other)
		{
			return !IsSameSide(other.Start, other.End) && !other.IsSameSide(Start, End);
		}

        /**
         * Get information on how the two lines interact.
         */
        public Relation GetRelation(LineSegment2D other, float tolerance = 0)
        {
            //see IsIntersecting...
            Vector2D v = StartToEnd;
            Vector2D ov = other.StartToEnd;
            Vector2D s = Start - other.Start;

            float denom = v.Cross(ov);
            float a = ov.Cross(s) / denom;
            float b = v.Cross(s) / denom;

            Relation result = Relation.NoConnection;
            if (Math.Abs(a - 1) < tolerance && (b >= 0 && b <= 1)) //touching - a is 1 and b in range
                result = result | Relation.EndConnects;
            if (Math.Abs(a) < tolerance && (b >= 0 && b <= 1)) //touching - a is 0 and b in range
                result = result | Relation.StartConnects;
            if (Math.Abs(b) < tolerance && (a >= 0 && a <= 1)) //touching - b is 1 and a in range
                result = result | Relation.OtherStartConnects;
            if (Math.Abs(b - 1) < tolerance && (a >= 0 && a <= 1)) //touching - b is 0 and a in range
                result = result | Relation.OtherEndConnects;
            if (a > tolerance && (a - 1) < tolerance && b > tolerance && (b - 1) < tolerance)
                result = result | Relation.Intersection;

            return result;
        }

        /**
         * Returns the sample ratio where this line would intersect the other. Segment boundaries are not considered.
         */
        public float GetIntersectionRatio(LineSegment2D other)
        {
            //see IsIntersecting...
            Vector2D ov = other.StartToEnd;
            Vector2D s = Start - other.Start;

            float denom = StartToEnd.Cross(ov);
            return ov.Cross(s) / denom;
        }

        public float GetIntersectionRatio(Vector2D start, Vector2D dir)
        {
            //see IsIntersecting...
            Vector2D s = Start - start;

            float denom = StartToEnd.Cross(dir);
            return dir.Cross(s) / denom;
        }


        /**
         * Returns a new vector v at the position where the lines would intersect. Segment boundaries are not considered.
         */
        public Vector2D Intersect(LineSegment2D other)
        {
            return Start + StartToEnd * GetIntersectionRatio(other);
        }

        /**
         * Project the vector v on this line segment and return the ratio.
         * return = 0      v = start
         * return = 1      v = end
         * return < 0      v is on the backward extension of AB
         * return > 1      v is on the forward extension of AB
         * 0< return <1    v is interior to AB
         */
        public float GetSnapRatio(Vector2D v)
        {
            /*    
            Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
            Let P be the point of perpendicular projection of C on AB.  The parameter
            r, which indicates P's position along AB, is computed by the dot product 
            of AC and AB divided by the square of the length of AB:
		    
            (1)     AC dot AB
                r = ---------  
                    ||AB||^2
            */
            return StartToEnd.Dot(v - Start) / LengthSquared;
        }

        /**
         * Returns a new vector on this line segment at the given 'ratio'. A ratio of 0 returns v1 and a ratio of 1 returns v2.
         */
        public Vector2D Sample(float r)
        {
            return Start + StartToEnd * r;
        }

        /**
         * Returns the point on this line segment that is closest to the vector.
         */
        public Vector2D Snapped(Vector2D v)
        {
            return Sample(Math.Max(0, Math.Min(1, GetSnapRatio(v))));
        }

        /**
         * Returns the distance of the vector to its closest point on this line segment.
         */
        public float Distance(Vector2D v)
        {
            return Snapped(v).Distance(v);
        }

        /**
         * Returns the square of the distance of the vector to its closest point on this line segment.
         */
        public float DistanceSquared(Vector2D v)
        {
            return Snapped(v).DistanceSquared(v);
        }

        public override string ToString()
        {
            return "(" + Start.ToString() + "->" + End.ToString() + ")";
        }
    }
}
