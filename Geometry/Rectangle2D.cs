/*******************************************************************************
 * Copyright (c) 2009-2012 by Thomas Jahn
 * Questions? Mail me at lithander@gmx.de!
 ******************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Vectrics
{
    public struct Rectangle2D
    {
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

        public Vector2D TopLeft
        {
            get { return new Vector2D(X,Y); }
            set 
            {
                float right = X + Width;
                X = value.X;
                Width = right - X;

                float bottom = Y + Height;
                Y = value.Y;
                Height = bottom - Y;
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

        public float Right
		{
			get { return X + Width; }
            set { Width = value - X; }
		}
		
        public float Bottom //setting top preserves bottom while X preserves Height
		{
			get { return Y + Height; }
            set { Height = value - Y ; }
        }

        public Vector2D BottomRight
        {
            get { return new Vector2D(X+Width, Y+Height); }
            set { Width = value.X - X; Height = value.Y - Y; }
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
		
		/**
		 * Assigns new coordinates and dimension to this rectangle.
		 */
		public void Set(float x = 0, float y = 0, float w = 0, float h = 0 )
		{
			X = x;
			Y = y;
			Width = w;
			Height = h;
		}

        public void Set(Vector2D topLeft, Vector2D size)
        {
            X = topLeft.X;
            Y = topLeft.Y;
            Width = size.X;
            Height = size.Y;
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
         * Enlarges the rectangle to include the position.
         */
        public void Add(Vector2D v)
        {
            if (v.X < X)
            {
                Width += X - v.X;
                X = v.X;
            }
            if (v.X > X + Width)
                Width = v.X - X;

            if (v.Y < Y)
            {
                Height += Y - v.Y;
                Y = v.Y;
            }
            if (v.Y > Y + Height)
                Height = v.Y - Y;
        }

        public void Add(float x, float y)
        {
            if (x < X)
            {
                Width += X - x;
                X = x;
            }
            if (x > X + Width)
                Width = x - X;

            if (y < Y)
            {
                Height += Y - y;
                Y = y;
            }
            if (y > Y + Height)
                Height = y - Y;
        }

        public void Add(Rectangle2D r)
        {
            if (r.X < X)
            {
                Width += X - r.X;
                X = r.X;
            }
            if (r.X + r.Width > X + Width)
                Width += r.X - X;

            if (r.Y < Y)
            {
                Height += Y - r.Y;
                Y = r.Y;
            }
            if (r.Y > Y + Height)
                Height += r.Y - Y;
        }
		
		/**
		 * Tests if the rectangle includes the vector.
		 */
		public bool Contains(Vector2D v)
		{
			if(v.X < X)
				return false;
			if(v.Y < Y)
				return false;
			if(v.X > (X + Width))
				return false;
			if(v.Y > (Y + Height))
				return false;
			
			return true;
		}

   		/**
		 * Tests if the rectangle includes the vector.
		 */
		public bool Contains(float x, float y)
		{
			if(x < X)
				return false;
			if(y < Y)
				return false;
			if(x > (X + Width))
				return false;
			if(y > (Y + Height))
				return false;
			
			return true;
		}

        /**
		 * Returns true if rect is completely outside of this;
		 */
		public bool IsOutside(Rectangle2D rect)
		{
			return (rect.X > X+Width) || (rect.Y > Y+Height) || (rect.Right < X) || (rect.Bottom < Y);
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

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Width.GetHashCode();
            hash = hash * 23 + Height.GetHashCode();
            return hash;
        }
        
		/**
		 * Enlarges the rectangle by margin in all directions
		 */
		public void ApplyMargin(float margin)
		{
			X -= margin;
			Y -= margin;
			Width += 2*margin;
			Height += 2*margin;
		}

        /**
		 * Enlarges the rectangle by margin
		 */
        public void ApplyMargin(float marginX, float marginY)
        {
            X -= marginX;
            Y -= marginY;
            Width += 2 * marginX;
            Height += 2 * marginY;
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
        public static Rectangle2D operator *(Rectangle2D r, Vector2D v)
        {
            Rectangle2D result = r;
            result.X *= v.X;
            result.Y *= v.Y;
            result.Width *= v.X;
            result.Height *= v.Y;
            return result;
        }
				
		/**
		 * Enlarges the rectangle to include the vector.
		 */
        public static Rectangle2D operator+(Rectangle2D r, Vector2D v)
        {
            Rectangle2D result = r;
            if(v.X < result.X)
			{
				result.Width += result.X - v.X;
				result.X = v.X;
			}
			if(v.X > result.X+result.Width)
				result.Width += v.X-result.X;
			
			if(v.Y < result.Y)
			{
				result.Height += result.Y - v.Y;
				result.Y = v.Y;
			}
			if(v.Y > result.Y+result.Height)
				result.Height += v.Y-result.Y;

            return result;
		}
		
		/**
		 * Enlarges the rectangle to include the rect.
		 */
        public static Rectangle2D operator+(Rectangle2D r1, Rectangle2D r2)
        {
            Rectangle2D result = r1;
            if(r2.X < result.X)
			{
				result.Width += result.X - r2.X;
				result.X = r2.X;
			}
			if(r2.X+r2.Width > result.X+result.Width)
				result.Width += r2.X-result.X;
			
			if(r2.Y < result.Y)
			{
				result.Height += result.Y - r2.Y;
				result.Y = r2.Y;
			}
			if(r2.Y > result.Y+result.Height)
				result.Height += r2.Y-result.Y;

            return result;
		}

		/**
		 * Returns the point on or within this rectangle that is closest to the vector.
		 */
		public Vector2D Snapped(Vector2D v)
		{
            Vector2D result;
			result.X = Math.Max(X, Math.Min(X+Width, v.X));
			result.Y = Math.Max(Y, Math.Min(Y+Height, v.Y));
			return v;
		}

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
			float xmax = X + Width;
			float ymax = Y + Height;
			
			float u1 = 0.0f;
			float u2 = 1.0f;
			
			float deltaX = (line.End.X - line.Start.X);
			float deltaY = (line.End.Y - line.Start.Y);

			//ugly but optimized
			//left
			float q = line.Start.X - X;
			if( deltaX == 0 && q < 0 )
				return null;
			float r = q / (-deltaX);
			if(deltaX > 0 )
				u1 = Math.Max(u1, r);
			else if( deltaX < 0 )
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
				return null;					
			
            //right
			q = xmax - line.Start.X;
			if( deltaX == 0 && q < 0 )
				return null;
			r = q / deltaX;
			if( deltaX < 0 )
				u1 = Math.Max(u1, r);
			else if( deltaX > 0 )
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
				return null;					
			//top
			q = line.Start.Y - Y;
			if( deltaY == 0 && q < 0 )
				return null;
			r = q / -deltaY;
			if(deltaY > 0 )
				u1 = Math.Max(u1, r);
			else if( deltaY < 0 )
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
				return null;					
			//bottom
			q = ymax - line.Start.Y;
			if( deltaY == 0 && q < 0 )
				return null;
			r = q / deltaY;
			if( deltaY < 0 )
				u1 = Math.Max(u1, r);
			else if( deltaY > 0 )
				u2 = Math.Min(u2, r);
			if( u1 > u2 )
				return null;				
		
			//clip
			if( u2 < 1 )
				line.End = new Vector2D(
                    line.Start.X + u2 * deltaX,
				    line.Start.Y + u2 * deltaY
                );

            if( u1 > 0)
				line.Start = new Vector2D(
                    line.Start.X + u1 * deltaX, 
                    line.Start.Y + u1 * deltaY
                );

			return line;    	
		}
    }
}
