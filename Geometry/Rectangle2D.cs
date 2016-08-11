/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public struct Rectangle2D : IEquatable<Rectangle2D>
    {
        public static Rectangle2D Zero = new Rectangle2D(0, 0, 0, 0);
        public static Rectangle2D Void = new Rectangle2D(float.NaN, float.NaN, float.NaN, float.NaN);

        public enum RectRelation 
        {
		    RECT_AROUND = -2,
		    RECT_INSIDE = -1,
		    RECT_INTERSECTS = 0,
		    RECT_OUTSIDE_LEFT = 1,			
		    RECT_OUTSIDE_TOP = 2,
			RECT_OUTSIDE_TOP_LEFT = 3,			
		    RECT_OUTSIDE_RIGHT = 4,			
			RECT_OUTSIDE_TOP_RIGHT = 5,			
		    RECT_OUTSIDE_BOTTOM = 8,
		    RECT_OUTSIDE_BOTTOM_LEFT = 9,
            RECT_OUTSIDE_BOTTOM_RIGHT = 12
	    };

        public enum LineRelation
        {
            LINE_INSIDE = -1,
		    LINE_INTERSECTS = 0,
		    LINE_OUTSIDE = 1	
        }
        
		/**
		 * The x coordinate of the topleft edge of the rectangle.
		 */
		public float X;
		
		/**
		 * The x coordinate of the topleft edge of the rectangle.
		 */
		public float Y;
		
		/**
		 * The width of the rectangle.
		 */
		public float Width;
		
		/**
		 * The height of the rectangle.
		 */
		public float Height;

        public float Perimeter
        {
            get 
            { 
                return 2 * (Width + Height);
            }
        }

        public Vector2D Size
        {
            get { return new Vector2D(Width, Height); }
        }

        public Vector2D Center
        {
            get { return new Vector2D(X + Width / 2, Y + Height / 2); }
        }
		
		public float Left //setting Left preserves Bottom while settign X preserves Height
		{
            get { return X; }
            set 
            { 
                float right = X + Width;
                X = value;
                Width = right - X;
            }
		}

   		public float Top //setting Top preserves Bottom while settign X preserves Height
		{
			get { return Y; }
            set 
            { 
                float bottom = Y + Height;
                Y = value;
                Height = bottom - Y;
            }
        }        

        public float Right
        {
            get { return X + Width; }
            set { Width = value - X; }
        }

        public float Bottom
        {
            get { return Y + Height; }
            set { Height = value - Y; }
        }

        public Vector2D TopLeft
        {
            get { return new Vector2D(X,Y); }
            set { Left = value.X; Top = value.Y; }
        }

        public Vector2D TopRight
        {
            get { return new Vector2D(X + Width, Y); }
            set { Width = value.X - X; Top = value.Y; }
        }

        public Vector2D BottomRight
        {
            get { return new Vector2D(X + Width, Y + Height); }
            set { Width = value.X - X; Height = value.Y - Y; }
        }

        public Vector2D BottomLeft
        {
            get { return new Vector2D(X, Y + Height); }
            set { Left = value.X; Height = value.Y - Y; }
        }

		/**
		 * The area of this rectangle
		 */
		public float Area
		{
			get { return Width * Height; }
		}
		
		/**
		 * Get a string representation of this rectangle
		 */
		public override string ToString()
		{
			return "(x=" + X + ", y=" + Y + ", width=" + Width + ", height=" + Height + ")";
		}

        /**
		 * Constructor
		 */
		public Rectangle2D( float x = 0, float y = 0, float w = 0, float h = 0 )
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}

        public Rectangle2D(Vector2D topLeft, Vector2D size)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = size.X;
            Height = size.Y;
        }	
		
        public static Rectangle2D FromPoints(Vector2D p1, Vector2D p2)
        {
            return new Rectangle2D(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        public static Rectangle2D FromPoints(float x1, float y1, float x2, float y2)
        {
            return new Rectangle2D(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x1 - x2), Math.Abs(y1 - y2));
        }

        public static Rectangle2D FromCenterRadius(Vector2D center, float radius)
        {
            return new Rectangle2D(center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
        }

        public void Constrain(Rectangle2D rect)
        {
            Left = Math.Max(rect.Left, Left);
            Top = Math.Max(rect.Top, Top);
            Right = Math.Min(rect.Right, Right);
            Bottom = Math.Min(rect.Bottom, Bottom);
        }

        public void Constrain(float left, float top, float right, float bottom)
        {
            Left = Math.Max(left, Left);
            Top = Math.Max(top, Top);
            Right = Math.Min(right, Right);
            Bottom = Math.Min(bottom, Bottom);
        }

        /**
         * Enlarges the rectangle to include the rectangle
         */
        public Rectangle2D Union(Rectangle2D r)
        {
            return Rectangle2D.FromPoints(Math.Min(X, r.X), Math.Min(Y, r.Y), Math.Max(X + Width, r.X + r.Width), Math.Max(Y + Height, r.Y + r.Height));
        }

        public Rectangle2D Union(Vector2D v)
        {
            return Rectangle2D.FromPoints(Math.Min(X, v.X), Math.Min(Y, v.Y), Math.Max(X + Width, v.X), Math.Max(Y + Height, v.Y));
        }

        public Rectangle2D Union(float x, float y)
        {
            return Rectangle2D.FromPoints(Math.Min(X, x), Math.Min(Y, y), Math.Max(X + Width, x), Math.Max(Y + Height, y));
        }

        public Rectangle2D Intersection(Rectangle2D rect)
        {
            if (Overlaps(rect))
                return FromPoints(Snapped(rect.TopLeft), Snapped(rect.BottomRight));
            else
                return Rectangle2D.Zero;
        }

		/**
		 * Tests if the rectangle includes the vector.
		 */
		public bool Contains(Vector2D v)
		{
            return v.X >= X && v.Y >= Y && v.X <= (X + Width) && v.Y <= (Y + Height);
            //return v.X >= X && v.Y >= Y && v.X <= Right && v.Y <= Bottom;
		}

   		/**
		 * Tests if the rectangle includes the vector.
		 */
		public bool Contains(float x, float y)
		{
            return x >= X && y >= Y && x <= (X + Width) && y <= (Y + Height);
		}

        /**
         * Tests if the rectangle includes the rectangle.
         */
        public bool Contains(Rectangle2D rect)
        {
            return rect.X >= X && rect.Y >= Y && (rect.X + rect.Width) <= (X + Width) && (rect.Y + rect.Height) <= (Y + Height);
        }

        /**
         * Tests if the rectangle shares space with the other rectangle.
         */
        public bool Overlaps(Rectangle2D rect)
        {
            return X <= (rect.X + rect.Width) && Y <= (rect.Y + rect.Height) && (X + Width) >= rect.X && (Y + Height) >= rect.Y;
            //return X < rect.Right && Y < rect.Bottom && Right > rect.X && Bottom > rect.Y;
        }      

        public bool Overlaps(LineSegment2D line)
        {
            //same aproach as distance to line segment but simplified
            //snap the 4 corners to the line and form delta
            Vector2D d1 = line.SnapDelta(TopLeft);
            Vector2D d2 = line.SnapDelta(TopRight);
            Vector2D d3 = line.SnapDelta(BottomLeft);
            Vector2D d4 = line.SnapDelta(BottomRight);
            if (CgMath.Sign(d1.X, d2.X, d3.X, d4.X) == 0)//mixed signs
                return true;

            if (CgMath.Sign(d1.Y, d2.Y, d3.Y, d4.Y) == 0)//mixed signs
                return true;

            return false;
        }

        public static bool operator==(Rectangle2D r1, Rectangle2D r2)
        {
            return (r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height);
        }

        public static bool operator!=(Rectangle2D r1, Rectangle2D r2)
        {
            return (r1.X != r2.X || r1.Y != r2.Y || r1.Width != r2.Width || r1.Height != r2.Height);
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;
            Rectangle2D r = (Rectangle2D)obj;
            return X == r.X && Y == r.Y && Width == r.Width && Height == r.Height;
        }
        
        public bool Equals(Rectangle2D r)
        {
            return X == r.X && Y == r.Y && Width == r.Width && Height == r.Height;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Width.GetHashCode();
            hash = hash * 23 + Height.GetHashCode();
            return hash;
        }

        public Vector2D InRectSpace(Vector2D v)
        {
            return new Vector2D(
                CgMath.Clamp(v.X - X, 0, Width),
                CgMath.Clamp(v.Y - Y, 0, Height));
        }
        
		/**
		 * Enlarges the rectangle by margin in all directions (CAUTION: negative margin can destroy the rect if its not large enough)
		 */
		public void Expand(float margin)
		{
			X -= margin;
			Y -= margin;
			Width += 2*margin;
			Height += 2*margin;
		}

        /**
		 * Enlarges the rectangle by margin (CAUTION: negative margin can destroy the rect if its not large enough)
		 */
        public void Expand(float marginX, float marginY)
        {
            X -= marginX;
            Y -= marginY;
            Width += 2 * marginX;
            Height += 2 * marginY;
        }

        /**
         * Enlarges the rectangle by margin
         */
        public void Expand(float left, float top, float right, float bottom)
        {
            X -= left;
            Y -= top;
            Width += left + right;
            Height += top + bottom;
        }

        /**
         * Enlarges the rectangle by margin in all directions
         */
        public Rectangle2D Expanded(float margin)
        {
            return new Rectangle2D(X - margin, Y - margin, Width + 2 * margin, Height + 2 * margin);
        }

        /**
		 * Enlarges the rectangle by margin
		 */
        public Rectangle2D Expanded(float marginX, float marginY)
        {
            return new Rectangle2D(X - marginX, Y - marginY, Width + 2 * marginX, Height + 2 * marginY);
        }

        /**
         * Enlarges the rectangle by margin
         */
        public Rectangle2D Expanded(float left, float top, float right, float bottom)
        {
            return new Rectangle2D(X - left, Y - top, Width + left + right, Height + top + bottom);
        }

        public void ShrinkToGrid(float gridSpacing)
        {
            double x2 = gridSpacing * Math.Floor((X + Width) / gridSpacing);
            double y2 = gridSpacing * Math.Floor((Y + Height) / gridSpacing);
            X = gridSpacing * (float)Math.Ceiling(X / gridSpacing);
            Y = gridSpacing * (float)Math.Ceiling(Y / gridSpacing);
            Width = (float)(x2 - X);
            Height = (float)(y2 - Y);
        }

        public void ExpandToGrid(float gridSpacing)
        {
            double x2 = gridSpacing * Math.Ceiling((X + Width) / gridSpacing);
            double y2 = gridSpacing * Math.Ceiling((Y + Height) / gridSpacing);
            X = gridSpacing * (float)Math.Floor(X / gridSpacing);
            Y = gridSpacing * (float)Math.Floor(Y / gridSpacing);
            Width = (float)(x2 - X);
            Height = (float)(y2 - Y);
        }

        /**
		 * Scales the rectangle by a vector.
		 */
        public static Rectangle2D operator*(Rectangle2D r, Vector2D v)
        {
            return new Rectangle2D(r.TopLeft * v, r.Size * v) ;
        }

        public static Rectangle2D operator*(Rectangle2D r, float s)
        {
            return new Rectangle2D(r.TopLeft * s, r.Size * s);
        }

        /**
         * Scales the rectangle by inverse of a vector.
         */
        public static Rectangle2D operator /(Rectangle2D r, Vector2D v)
        {
            return new Rectangle2D(r.TopLeft / v, r.Size / v);
        }

        public static Rectangle2D operator /(Rectangle2D r, float s)
        {
            return new Rectangle2D(r.TopLeft / s, r.Size / s);
        }
				
		/**
		 * Enlarges the rectangle to include the vector.
		 */
        public static Rectangle2D operator+(Rectangle2D r, Vector2D v)
        {
            return r.Union(v);
		}
		
		/**
		 * Enlarges the rectangle to include the rect.
		 */
        public static Rectangle2D operator+(Rectangle2D r1, Rectangle2D r2)
        {
            return r1.Union(r2);
		}

		/**
		 * Returns the point on or within this rectangle that is closest to the vector.
		 */
		public Vector2D Snapped(Vector2D v)
		{
            v.X = Math.Max(X, Math.Min(X+Width, v.X));
            v.Y = Math.Max(Y, Math.Min(Y+Height, v.Y));
            return v;
        }

        public float Distance(Vector2D v)
        {
            float dX = Math.Max(X - v.X, Math.Max(v.X - X - Width, 0));
            float dY = Math.Max(Y - v.Y, Math.Max(v.Y - Y - Height, 0));
            return (float)Math.Sqrt((dX*dX)+(dY*dY));
        }

        public float DistanceSquared(Vector2D v)
        {
            float dX = Math.Max(X - v.X, Math.Max(v.X - X - Width, 0));
            float dY = Math.Max(Y - v.Y, Math.Max(v.Y - Y - Height, 0));
            return dX * dX + dY * dY;
        }

        public float DistanceSquared(LineSegment2D line)
        {
            //snap the 4 corners to the line and form delta
            Vector2D d1 = line.SnapDelta(TopLeft);
            Vector2D d2 = line.SnapDelta(TopRight);
            Vector2D d3 = line.SnapDelta(BottomLeft);
            Vector2D d4 = line.SnapDelta(BottomRight);
            float xLower = Math.Min(Math.Min(d1.X, d2.X), Math.Min(d3.X, d4.X));
            float xUpper = Math.Max(Math.Max(d1.X, d2.X), Math.Max(d3.X, d4.X));
            float dX = (xUpper * xLower < 0) ? 0 : Math.Min(Math.Abs(xLower), Math.Abs(xUpper)); //if not all on the same side distance along X axis is 0

            float yLower = Math.Min(Math.Min(d1.Y, d2.Y), Math.Min(d3.Y, d4.Y));
            float yUpper = Math.Max(Math.Max(d1.Y, d2.Y), Math.Max(d3.Y, d4.Y));
            float dY = (yUpper * yLower < 0) ? 0 : Math.Min(Math.Abs(yLower), Math.Abs(yUpper)); //if not all on the same side distance along Y axis is 0
            return dX * dX + dY * dY;
        }

        public float Distance(LineSegment2D line)
        {
            return (float)Math.Sqrt(DistanceSquared(line));
        }

        /*
        Rect2D lerp(const Rect2D& other, float alpha) const {
            Rect2D out;
        
            out.min = min.lerp(other.min, alpha);
            out.max = max.lerp(other.max, alpha);

            return out;
        }
        */

        /**
		 * Returns a combination of the values described in RectRelation
		 */
		public RectRelation DescribeRelation(Rectangle2D rect)
		{
			//OUTSIDE?
            int result = 0;
			if(X >= (rect.X + rect.Width))
				result += (int)RectRelation.RECT_OUTSIDE_LEFT;
			if(Y >= (rect.Y + rect.Height))
				result += (int)RectRelation.RECT_OUTSIDE_TOP;
			if((X+Width) <= rect.X)
				result += (int)RectRelation.RECT_OUTSIDE_RIGHT;
			if((Y+Height) <= rect.Y)	
				result += (int)RectRelation.RECT_OUTSIDE_BOTTOM;
			
			if(result > 0)
				return (RectRelation)result;
			
			bool leftInside = (rect.X > X);
			bool topInside = (rect.Y > Y);
			bool rightInside = (rect.Right < X+Width);
			bool bottomInside = (rect.Bottom < Y+Height);

			if(leftInside && topInside && rightInside && bottomInside)
				return RectRelation.RECT_INSIDE;
			
			if(leftInside || topInside || rightInside || bottomInside)
				return RectRelation.RECT_INTERSECTS;

			return RectRelation.RECT_AROUND;
		}
        
		/**
		 * Returns 1 if inside, 0 if intersecting and -1 outside.
		 */
		public LineRelation DescribeRelation(LineSegment2D line)
		{
			//int x1, int y1, int x2, int y2, 
			float xmax = X + Width;
			float ymax = Y + Height;
			
			float u1 = 0.0f;
			float u2 = 1.0f;
			
			float deltaX = (line.End.X - line.Start.X);
            float deltaY = (line.End.Y - line.Start.Y);
			
			/*
			* left edge, right edge, bottom edge and top edge checking
			*/
			float[] pPart = {-deltaX, deltaX, -deltaY, deltaY};
            float[] qPart = { line.Start.X - X, xmax - line.Start.X, line.Start.Y - Y, ymax - line.Start.Y };
			
			for( int i = 0; i < 4; i ++ )
			{
				float p = pPart[ i ];
				float q = qPart[ i ];
				
				if( p == 0 && q < 0 )
					return LineRelation.LINE_OUTSIDE;
				
				float r = q / p;
				
				if( p < 0 )
					u1 = Math.Max(u1, r);
				else if( p > 0 )
					u2 = Math.Min(u2, r);
				
				if( u1 > u2 )
					return LineRelation.LINE_OUTSIDE;					
			}
			
			if( u2 < 1 || u1 < 1 )
				return LineRelation.LINE_INTERSECTS;
			else
				return LineRelation.LINE_INSIDE;
		}

        /**
         * Constrains a linesegment to fit in the rectangle. Uses Liang Barsky Algorithm. 
         * Returns reference to the passed input argument after clipping or null if no portion of the line is within the rectangle.
         */
		public LineSegment2D Clip(LineSegment2D line)
		{
   			//int x1, int y1, int x2, int y2, 
            Vector2D max = BottomRight;
            Vector2D start = line.Start;
            Vector2D delta = line.StartToEnd;
			
			float u1 = 0.0f;
			float u2 = 1.0f;

            if (delta.X == 0 && (start.X < X || start.X > max.X))
                return LineSegment2D.Void;
            
            if (delta.Y == 0 && (start.Y < Y || start.Y > max.Y))
                return LineSegment2D.Void;

			//ugly but optimized
			//left
            float r = (start.X - X) / (-delta.X);
            if (delta.X > 0)
				u1 = Math.Max(u1, r);
            else if (delta.X < 0)
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
                return LineSegment2D.Void;					
			
            //right
            r = (max.X - start.X) / delta.X;
            if (delta.X < 0)
				u1 = Math.Max(u1, r);
            else if (delta.X > 0)
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
                return LineSegment2D.Void;					
			
            //top
            r = (start.Y - Y) / -delta.Y;
            if (delta.Y > 0)
				u1 = Math.Max(u1, r);
            else if (delta.Y < 0)
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
                return LineSegment2D.Void;					
			
            //bottom
            r = (max.Y - start.Y) / delta.Y;
            if (delta.Y < 0)
				u1 = Math.Max(u1, r);
            else if (delta.Y > 0)
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
                return LineSegment2D.Void;				
		
			//clip
			if( u2 < 1 )
				line.End = start + u2 * delta;

            if( u1 > 0)
				line.Start = start + u1 * delta;

			return line;    	
		}

    }
}
