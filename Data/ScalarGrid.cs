using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class ScalarGrid : Grid<float>
    {
        public ScalarGrid(Rectangle2D region, float spacing)
            : base(region, spacing) //resolution in cells per meter
        {

        }

        public enum SamplingMethod
        {
            Direct,
            Lerp,
            SmartLerp
        }

        public float Sample(Vector2D pt, SamplingMethod mode)
        {
            if (mode == SamplingMethod.Lerp)
                return SampleLerp(pt);
            else if (mode == SamplingMethod.SmartLerp)
                return SampleSmart(pt, 1.0f);
            else
                return Sample(pt);
        }

        public float SampleLerp(Vector2D point)
        {
            Probe probe;
            SampleProbe(point, out probe);
            float vx1 = probe.TopLeft + probe.U * (probe.TopRight - probe.TopLeft);
            float vx2 = probe.BottomLeft + probe.U * (probe.BottomRight - probe.BottomLeft);
            return vx1 + probe.V * (vx2 - vx1);
        }

        public float SampleSmart(Vector2D point, float threshold)
        {
            threshold = Math.Max(Spacing, threshold); //threshold can be smaller then spacing
            Probe probe;
            SampleProbe(point, out probe);
            //interpolate between the 4 corners (first 2x along X - then 1x along Y)
            float dxTop = (probe.TopRight - probe.TopLeft);
            float dxBottom = (probe.BottomRight - probe.BottomLeft);
            float dx = dxBottom * probe.V + (1 - probe.V) * dxTop;
            //the more the gradient dx approaches the threshold, the less we interpolate
            float sx = CgMath.Linstep(Spacing, threshold, Math.Abs(dx));
            float uSmart = CgMath.Lerp(probe.U, (float)Math.Round(probe.U), sx);
            float xTop = probe.TopLeft + uSmart * dxTop;
            float xBottom = probe.BottomLeft + uSmart * dxBottom;
            float dy = (xBottom - xTop);
            //the more the gradient approaches the threshold, the less we interpolate
            float sy = CgMath.Linstep(Spacing, threshold, Math.Abs(dy));
            float vSmart = CgMath.Lerp(probe.V, (float)Math.Round(probe.V), sy);
            return xTop + vSmart * dy;
        }
    }
}
