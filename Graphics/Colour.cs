using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vectrics;

namespace Vectrics
{
    public struct Colour
    {
        public static Colour Transparent = new Colour(0, 0, 0, 0);
        public static Colour White = new Colour(1, 1, 1, 1);
        public static Colour Black = new Colour(0, 0, 0, 1);
        public static Colour Blue = new Colour(0, 0, 1, 1);
        public static Colour Cyan = new Colour(0, 1, 1, 1);
        public static Colour Grey = new Colour(0.5f, 0.5f, 0.5f, 1);
        public static Colour Green = new Colour(0, 1, 0, 1);
        public static Colour Magenta = new Colour(1, 0, 1, 1);
        public static Colour Red = new Colour(1, 0, 0, 1);
        public static Colour Yellow = new Colour(1, 1, 0, 1);
        public static Colour Orange = new Colour(1, 0.7f, 0.1f, 1);


        public float R;
		public float G;
		public float B;
		public float A;
	
		public Colour(float r, float g, float b, float a = 1)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

        public Colour(uint color)
        {
            uint a = color >> 24 & 0xFF;
            uint r = color >> 16 & 0xFF;
            uint g = color >> 8 & 0xFF;
            uint b = color & 0xFF;
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;
            A = a / 255.0f; 
        }

        public Colour(float alpha)
        {
            R = 1;
            G = 1;
            B = 1;
            A = alpha;
        }

        public byte RedByte
        {
            get { return R < 0 ? (byte)0 : R > 1 ? (byte)255 : (byte)(R * 255.0); }
            set { R = value / 255.0f; }
        }

        public byte GreenByte
        {
            get { return G < 0 ? (byte)0 : G > 1 ? (byte)255 : (byte)(G * 255.0); }
            set { G = value / 255.0f; }
        }

        public byte BlueByte
        {
            get { return B < 0 ? (byte)0 : B > 1 ? (byte)255 : (byte)(B * 255.0); }
            set { B = value / 255.0f; }
        }
        
        public byte AlphaByte
        {
            get { return A < 0 ? (byte)0 : A > 1 ? (byte)255 : (byte)(A * 255.0); }
            set { A = value / 255.0f; }
        }

        public uint RGBA
        {
            get { return (uint)(AlphaByte << 24) | (uint)(RedByte << 16) | (uint)(GreenByte << 8) | (uint)(BlueByte); }
            set { SetRGBA(value); }
        }

        private void SetRGBA(uint value)
        {
            uint a = value >> 24 & 0xFF;
            uint r = value >> 16 & 0xFF;
            uint g = value >> 8 & 0xFF;
            uint b = value & 0xFF;
            R = r / 255.0f;
            G = g / 255.0f;
            B = b / 255.0f;
            A = a / 255.0f; 
        }

        //HSV
        /**
		* Set Hue [0..1], Saturation [0..1] and Value [0..1]
		*/	
		public void SetHSV(float hue, float saturation, float brightness, float alpha = 1.0f)
		{
			hue = CgMath.Saturate(hue);
            saturation = CgMath.Saturate(saturation);
            brightness = CgMath.Saturate(brightness);
            float hseg = 6 * hue;
			float c = saturation * brightness;
			float x = c * ( 1 - Math.Abs(hseg%2 - 1));
			int i = (int)Math.Floor(hseg);
			switch (i) {
				case 0: R = c; G = x; B = 0; break;
				case 1: R = x; G = c; B = 0; break;
				case 2: R = 0; G = c; B = x; break;
				case 3: R = 0; G = x; B = c; break;
				case 4: R = x; G = 0; B = c; break;
				default: R = c; G = 0; B = x; break;
			}
			float m = brightness - c;
			R += m;
			G += m;
			B += m;
            A = alpha;
		}


        //[0..1]
        public float Hue
        {
            get
            {
                float slice = 1f / 6; //60°
                float max = Math.Max(R, Math.Max(G, B));
                float min = Math.Min(R, Math.Min(G, B));
                
                if (max == R && G >= B)
                    return slice * (G - B) / (max - min);
                
                if (max == R && G < B)
                    return slice * (G - B) / (max - min) + 6 * slice;
                
                if (max == G)
                    return slice * (B - R) / (max - min) + 2 * slice;
                
                //if (max == Blue)
                return slice * (R - G) / (max - min) + 4 * slice;
            }
            set
            {
                SetHSV(value, Saturation, Brighness);
            }
        }

        //[0..1]
        public float Saturation
        {
            get
            {
                float max = Math.Max(R, Math.Max(G, B));
                float min = Math.Min(R, Math.Min(G, B));
                return (max == 0) ? 0.0f : (1.0f - (min / max));
            }
            set
            {
                SetHSV(Hue, value, Brighness);
            }
        }

        //[0..1]
        public float Brighness
        {
            get
            {
                return Math.Max(R, Math.Max(G, B));
            }
            set
            {
                SetHSV(Hue, Saturation, value);
            }
        }

        /**
		 * Set Hue [0..1], Saturation [0..1] and Value [0..1]
		 */
        public static Colour FromHSV(float hue, float saturation, float value, float alpha = 1.0f)
		{
            Colour result = Colour.White;
			result.SetHSV(hue, saturation, value, alpha);
			return result;
		}

        public static float HueFromVector(Vector2D v)
        {
            return Angle.NormalizeRad2(v.PolarAngleRadian - Angle.HalfPI) / Angle.TwoPI;
        }

        //arithmetics
        //Basic Arithmetic
        public static Colour operator -(Colour p)
        {
            return new Colour(-p.R, -p.G, -p.B, -p.A);
        }
        public static Colour operator +(Colour p1, Colour p2)
        {
            return new Colour(p1.R + p2.R, p1.G + p2.G, p1.B + p2.B, p1.A + p2.A);
        }
        public static Colour operator -(Colour p1, Colour p2)
        {
            return new Colour(p1.R - p2.R, p1.G - p2.G, p1.B - p2.B, p1.A - p2.A);
        }
        public static Colour operator *(Colour p1, float scale)
        {
            return new Colour(p1.R * scale, p1.G * scale, p1.B * scale, p1.A * scale);
        }
        public static Colour operator *(Colour p1, Colour p2)
        {
            return new Colour(p1.R * p2.R, p1.G * p2.G, p1.B * p2.B, p1.A * p2.A);
        }
        public static Colour operator /(Colour p1, float scale)
        {
            return new Colour(p1.R / scale, p1.G / scale, p1.B / scale, p1.A / scale);
        }
        public static Colour operator /(Colour p1, Colour p2)
        {
            return new Colour(p1.R / p2.R, p1.G / p2.G, p1.B / p2.B, p1.A / p2.A);
        }

        public static Colour FromHexString(string color)
        {
            Colour result = new Colour();
            if (string.IsNullOrEmpty(color) || color.Length != 7)
                return result;
            

            string r_string = color.Substring(1, 2);
            string g_string = color.Substring(3, 2);
            string b_string = color.Substring(5, 2);

            byte r = byte.Parse(r_string, System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(g_string, System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(b_string, System.Globalization.NumberStyles.HexNumber);

            result.RedByte = r;
            result.GreenByte = g;
            result.BlueByte = b;
            result.A = 1;

            return result;
        }

        public string ToHexString()
        {
            string result = "#";
            result += RedByte.ToString("X2");
            result += GreenByte.ToString("X2");
            result += BlueByte.ToString("X2");
            return result;
        }
		
        /**
		 * Get a string representation of this color
		 */
		public override string ToString()
		{
			return "(r=" + RedByte + ", g=" + GreenByte + ", b=" + BlueByte + ", a=" + AlphaByte + ")";
		}
    }
}
