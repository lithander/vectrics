using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class VectorGrid : Grid<Vector2D>
    {
        public VectorGrid(Rectangle2D region, float spacing)
            : base(region, spacing) //resolution in cells per meter
        {

        }

        public Vector2D SampleStepped(Vector2D point, float edge0, float edge1)
        {
            Vector2D dir = Sample(point);
            return dir.Sized(CgMath.Linstep(edge0, edge1, dir.Length));
        }

        public Vector2D SampleBlerp(Vector2D point)
        {
            Probe probe;
            SampleProbe(point, out probe);

            float tl = (1.0f - probe.U) * (1.0f - probe.V);
            float tr = probe.U * (1.0f - probe.V);
            float bl = (1.0f - probe.U) * probe.V;
            float br = probe.U * probe.V;
            
            Vector2D result = tl * probe.TopLeft +
                tr * probe.TopRight +
                bl * probe.BottomLeft +
                br * probe.BottomRight;

            return result;
        }

        public float SampleLength(Vector2D point)
        {
            Probe probe;
            SampleProbe(point, out probe);

            float tl = (1.0f - probe.U) * (1.0f - probe.V);
            float tr = probe.U * (1.0f - probe.V);
            float bl = (1.0f - probe.U) * probe.V;
            float br = probe.U * probe.V;

            float length = tl * probe.TopLeft.Length +
                tr * probe.TopRight.Length +
                bl * probe.BottomLeft.Length +
                br * probe.BottomRight.Length;

            return length;
        }               

        public Vector2D SampleNormalized(Vector2D point)
        {
            return SampleBlerp(point).SafeNormalized();
        }

        public Vector2D SampleSmooth(Vector2D point)
        {
            Probe probe;
            SampleProbe(point, out probe);

            float tl = (1.0f - probe.U) * (1.0f - probe.V);
            float tr = probe.U * (1.0f - probe.V);
            float bl = (1.0f - probe.U) * probe.V;
            float br = probe.U * probe.V;

            float length = tl * probe.TopLeft.Length +
                tr * probe.TopRight.Length +
                bl * probe.BottomLeft.Length +
                br * probe.BottomRight.Length;
            
            Vector2D result = tl * probe.TopLeft +
                tr * probe.TopRight +
                bl * probe.BottomLeft +
                br * probe.BottomRight;

            return result.Sized(length);
        }

        public Vector2D SampleSmoothStepped(Vector2D point, float edge0, float edge1)
        {
            Probe probe;
            SampleProbe(point, out probe);

            float tl = (1.0f - probe.U) * (1.0f - probe.V);
            float tr = probe.U * (1.0f - probe.V);
            float bl = (1.0f - probe.U) * probe.V;
            float br = probe.U * probe.V;

            float length = tl * probe.TopLeft.Length +
                tr * probe.TopRight.Length +
                bl * probe.BottomLeft.Length +
                br * probe.BottomRight.Length;

            Vector2D result = tl * probe.TopLeft +
                tr * probe.TopRight +
                bl * probe.BottomLeft +
                br * probe.BottomRight;

            return result.Sized(CgMath.Linstep(edge0, edge1, length));
        }

    }
}
