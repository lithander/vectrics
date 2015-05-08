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

        public float Red;
		public float Green;
		public float Blue;
		public float Alpha;
	
		public Colour(float r, float g, float b, float a = 1)
		{
			Red = r;
			Green = g;
			Blue = b;
			Alpha = a;
		}

        public Colour(uint color)
        {
            uint a = color >> 24 & 0xFF;
            uint r = color >> 16 & 0xFF;
            uint g = color >> 8 & 0xFF;
            uint b = color & 0xFF;
            Red = r / 255.0f;
            Green = g / 255.0f;
            Blue = b / 255.0f;
            Alpha = a / 255.0f; 
        }

        public Colour(float alpha)
        {
            Red = 1;
            Green = 1;
            Blue = 1;
            Alpha = alpha;
        }

        public byte RedByte
        {
            get { return Red < 0 ? (byte)0 : Red > 1 ? (byte)255 : (byte)(Red * 255.0); }
            set { Red = value / 255.0f; }
        }

        public byte GreenByte
        {
            get { return Green < 0 ? (byte)0 : Green > 1 ? (byte)255 : (byte)(Green * 255.0); }
            set { Green = value / 255.0f; }
        }

        public byte BlueByte
        {
            get { return Blue < 0 ? (byte)0 : Blue > 1 ? (byte)255 : (byte)(Blue * 255.0); }
            set { Blue = value / 255.0f; }
        }
        
        public byte AlphaByte
        {
            get { return Alpha < 0 ? (byte)0 : Alpha > 1 ? (byte)255 : (byte)(Alpha * 255.0); }
            set { Alpha = value / 255.0f; }
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
            Red = r / 255.0f;
            Green = g / 255.0f;
            Blue = b / 255.0f;
            Alpha = a / 255.0f; 
        }

        //HSV
        /**
		* Set Hue [0..1], Saturation [0..1] and Value [0..1]
		*/	
		public void SetHSV(float hue, float saturation, float brightness)
		{
			hue = CgMath.Saturate(hue);
            saturation = CgMath.Saturate(saturation);
            brightness = CgMath.Saturate(brightness);
            float hseg = 6 * hue;
			float c = saturation * brightness;
			float x = c * ( 1 - Math.Abs(hseg%2 - 1));
			int i = (int)Math.Floor(hseg);
			switch (i) {
				case 0: Red = c; Green = x; Blue = 0; break;
				case 1: Red = x; Green = c; Blue = 0; break;
				case 2: Red = 0; Green = c; Blue = x; break;
				case 3: Red = 0; Green = x; Blue = c; break;
				case 4: Red = x; Green = 0; Blue = c; break;
				default: Red = c; Green = 0; Blue = x; break;
			}
			float m = brightness - c;
			Red += m;
			Green += m;
			Blue += m;
		}


        //[0..2PI]
        public float Hue
        {
            get
            {
                float slice = Angle.PI / 3; //60°
                float max = Math.Max(Red, Math.Max(Green, Blue));
                float min = Math.Min(Red, Math.Min(Green, Blue));
                
                if (max == Red && Green >= Blue)
                    return slice * (Green - Blue) / (max - min);
                
                if (max == Red && Green < Blue)
                    return slice * (Green - Blue) / (max - min) + 6 * slice;
                
                if (max == Green)
                    return slice * (Blue - Red) / (max - min) + 2 * slice;
                
                //if (max == Blue)
                return slice * (Red - Green) / (max - min) + 4 * slice;
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
                float max = Math.Max(Red, Math.Max(Green, Blue));
                float min = Math.Min(Red, Math.Min(Green, Blue));
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
                return Math.Max(Red, Math.Max(Green, Blue));
            }
            set
            {
                SetHSV(Hue, Saturation, value);
            }
        }

        /**
		 * Set Hue [0..1], Saturation [0..1] and Value [0..1]
		 */
        public static Colour FromHSV(float hue, float saturation, float value)
		{
            Colour result = Colour.White;
			result.SetHSV(hue, saturation, value);
			return result;
		}

        //arithmetics
        //Basic Arithmetic
        public static Colour operator -(Colour p)
        {
            return new Colour(-p.Red, -p.Green, -p.Blue, -p.Alpha);
        }
        public static Colour operator +(Colour p1, Colour p2)
        {
            return new Colour(p1.Red + p2.Red, p1.Green + p2.Green, p1.Blue + p2.Blue, p1.Alpha + p2.Alpha);
        }
        public static Colour operator -(Colour p1, Colour p2)
        {
            return new Colour(p1.Red - p2.Red, p1.Green - p2.Green, p1.Blue - p2.Blue, p1.Alpha - p2.Alpha);
        }
        public static Colour operator *(Colour p1, float scale)
        {
            return new Colour(p1.Red * scale, p1.Green * scale, p1.Blue * scale, p1.Alpha * scale);
        }
        public static Colour operator *(Colour p1, Colour p2)
        {
            return new Colour(p1.Red * p2.Red, p1.Green * p2.Green, p1.Blue * p2.Blue, p1.Alpha * p2.Alpha);
        }
        public static Colour operator /(Colour p1, float scale)
        {
            return new Colour(p1.Red / scale, p1.Green / scale, p1.Blue / scale, p1.Alpha / scale);
        }
        public static Colour operator /(Colour p1, Colour p2)
        {
            return new Colour(p1.Red / p2.Red, p1.Green / p2.Green, p1.Blue / p2.Blue, p1.Alpha / p2.Alpha);
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
            result.Alpha = 1;

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
