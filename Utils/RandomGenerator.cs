using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics.Utils
{
    public class RandomGenerator
    {
        private uint m_z;
        public uint Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        private uint m_w;
        public uint W
        {
            get { return m_w; }
            set { m_w = value; }
        }

        public RandomGenerator() : this(DateTime.Now.Millisecond) { }

        public RandomGenerator(int seed)
        {
            m_z = (uint)seed;
            m_w = 42;
        }

        public void Reseed(uint z, uint w = 42)
        {
            m_z = z;
            m_w = w;
        }

        private uint InternalNext()
        {
            m_z = 36969 * (m_z & 65535) + (m_z >> 16);
            m_w = 18000 * (m_w & 65535) + (m_w >> 16);
            uint next = (m_z << 16) + m_w;
            return next;
        }

        public Vector2D NextVector(float minLength, float maxLength)
        {
            float angle = NextFloat(Angle.TwoPI);
            float length = NextFloat(minLength, maxLength);
            return Vector2D.FromPolarRadian(length, angle);
        }

        public int Next()
        {
            return (int)(InternalNext() % int.MaxValue);
        }

        public int Next(int max)
        {
            return (int)(InternalNext() % max);
        }

        public int Next(int min, int max)
        {
            return (int)(InternalNext() % (max - min)) + min;
        }

        public double NextDouble()
        {
            return InternalNext() / (double)int.MaxValue;
        }

        public double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        public double NextDouble(double min, double max)
        {
            return NextDouble() * (max - min) + min;
        }
        
        public float NextFloat()
        {
            return (float)NextDouble();
        }

        public float NextFloat(float max)
        {
            return (float)NextDouble(max);
        }

        public float NextFloat(double min, double max)
        {
            return (float)NextDouble(min, max);
        }

        public bool NextBool()
        {
            return NextDouble() <= 0.5;
        }
        
        public bool NextBool(double chance)
        {
            return NextDouble() <= chance;
        }

        public T PickOne<T>(List<T> items)
        {
            if (items.Count > 0)
                return items[Next(items.Count)];
            else
                return default(T);
        }

        public T PickOne<T>(params T[] items)
        {
            return items[Next(items.Length)];
        }
    }
}
