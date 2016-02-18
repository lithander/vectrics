/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public struct LineSegment2D
    {
        public static LineSegment2D Zero = new LineSegment2D();
        public static LineSegment2D Void = new LineSegment2D(Vector2D.Void, Vector2D.Void);

        public bool IsVoid
        {
            get { return Start.IsVoid || End.IsVoid; }
        }
        
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
                End = Start + value;
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
                Start = End - value;
            }
        }

        public Vector2D Center
        {
            get
            {
                return 0.5f * (Start + End);
            }

            set
            {
                Vector2D delta = value - Center;
                Start += delta;
                End += delta;
            }
        }

        //start -> end normalized
        public Vector2D Direction
        {
            get { return (End - Start).SafeNormalized(); }
        }

        public LineSegment2D(Vector2D startPoint, Vector2D endPoint)
        {
            Start = startPoint;
            End = endPoint;
        }

        public LineSegment2D(float x, float y, float x2, float y2)
        {
            Start = new Vector2D(x, y);
            End = new Vector2D(x2, y2);
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

        public static bool operator ==(LineSegment2D a, LineSegment2D b)
        {
            return (a.Start == b.Start && a.End == b.End);
        }

        public static bool operator !=(LineSegment2D a, LineSegment2D b)
        {
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

        public enum Side
        {
            Left = -1,
            Colinear = 0,
            Right = 1
        }

        public Side GetSide(Vector2D pt)
        {
            return (Side)Math.Sign(StartToEnd.Cross(pt - Start));
            /*
            //its the basic idea of the crossproduct in R3 with a garuanteed 0 for z.
            return (Side)Math.Sign(Start.X * (End.Y - pt.Y) + End.X * (pt.Y - Start.Y) + pt.X * (Start.Y - End.Y));*/
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

        /**
         * Compare this line segment to another and return true if the lines have an intersection.
         */
        public bool IsIntersecting(LineSegment2D other)
        {
            int a = (int)GetSide(other.Start) * (int)GetSide(other.End);
            int b = (int)other.GetSide(Start) * (int)other.GetSide(End);
            return a == -1 && b == -1;
        }

        /**
        * Compare this line segment to another and return true if the lines are parallel.
        */
        public bool IsParallel(LineSegment2D other, float tolerance = 0)
        {
            return Math.Abs(StartToEnd.Cross(other.StartToEnd)) <= tolerance;
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
            Vector2D toEnd = End - Start;
            float sr = toEnd.Dot(v - Start) / toEnd.LengthSquared;
            return Start + toEnd * CgMath.Saturate(sr);
        }

        /**
         * Returns the point on this line segment that is closest to the vector.
         */
        public Vector2D SnapDelta(Vector2D v)
        {
            Vector2D toEnd = End - Start;
            float sr = toEnd.Dot(v - Start) / toEnd.LengthSquared;
            return Start + toEnd * CgMath.Saturate(sr) - v;
        }

        /**
         * Returns the distance of the vector to its closest point on this line segment.
         */
        public float Distance(Vector2D v)
        {
            return SnapDelta(v).Length;
        }

        /**
         * Returns the square of the distance of the vector to its closest point on this line segment.
         */
        public float DistanceSquared(Vector2D v)
        {
            return SnapDelta(v).LengthSquared;
        }

        /**
         * Returns the square of the distance of the vector to its closest point on this line segment.
         */
        public float Distance(LineSegment2D other)
        {
            if (IsIntersecting(other))
                return 0;//early out

            //see IsIntersecting...
            Vector2D v = StartToEnd;
            Vector2D ov = other.StartToEnd;
            Vector2D s = Start - other.Start;

            float denom = v.Cross(ov);
            if(denom == 0) //lines are parallel
                return Direction.OrthogonalizedClockwise().Dot(s);

            float a = CgMath.Saturate(ov.Cross(s) / denom);
            float b = CgMath.Saturate(v.Cross(s) / denom);
            return Math.Min(other.Distance(Sample(a)), this.Distance(other.Sample(b)));
        }

        public float DistanceSquared(LineSegment2D other)
        {
            //see IsIntersecting...
            Vector2D v = StartToEnd;
            Vector2D ov = other.StartToEnd;
            Vector2D s = Start - other.Start;

            float denom = v.Cross(ov);
            if (denom == 0) //lines are parallel
            {
                float d = Direction.OrthogonalizedClockwise().Dot(s);
                return d * d;
            }

            float a = CgMath.Saturate(ov.Cross(s) / denom);
            float b = CgMath.Saturate(v.Cross(s) / denom);
            return Math.Min(other.DistanceSquared(Sample(a)), this.DistanceSquared(other.Sample(b)));
        }

        public override string ToString()
        {
            return "(" + Start.ToString() + "->" + End.ToString() + ")";
        }
    }
}
