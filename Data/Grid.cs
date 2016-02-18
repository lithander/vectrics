using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class Grid<T>
    {
        private float _cellRadius;
        private float _spacing;
        private float _toGrid;
        private Rectangle2D _region;
        private Rectangle2D _innerRegion;
        private List<T> _data = null;
        private int _stride;
        private int _rows;
        private int _count;

        public struct CellRegion
        {
            public int Start;
            public int Right;
            public int Down;
        }

        //PUBLIC

        public int CellCount
        {
            get { return _count; }
        }

        public float Spacing
        {
            get { return _spacing; }
        }

        public float CellRadius
        {
            get { return _cellRadius; }
        }

        public Rectangle2D Region
        {
            get { return _region; }
        }

        public int Stride
        {
            get { return _stride; }
        }

        public Grid(Rectangle2D region, float spacing) //resolution in cells per meter
        {
            _spacing = spacing;
            _cellRadius = (float)(0.5 * Math.Sqrt(2) * _spacing);
            _toGrid = 1.0f / _spacing;
            _region = region;
            _region.ExpandToGrid(_spacing);
            _innerRegion = _region.Expanded(-0.01f);
            Vector2D topLeft = CellsCenters.First();
            _stride = CellsCenters.Where(cc => cc.Y == topLeft.Y).Count();
            _count = CellsCenters.Count();
            _rows = _count / _stride;
            _data = Enumerable.Repeat(default(T), _count).ToList();
        }

        public void Clear(T value)
        {
            int max_i = _data.Count;
            for (int i = 0; i < max_i; i++)
                _data[i] = value;
        }
        

        public T Sample(Vector2D point)
        {
            int index = CellIndexClamped(point);
            return _data[index];
        }

        public IEnumerable<T> SampleNeighbours(Vector2D point)
        {
            int i = CellIndexClamped(point);
            yield return _data[Math.Min(_stride - 1, Math.Max(0, i + 1))];
            yield return _data[Math.Min(_stride - 1, Math.Max(0, i + 1))];
            yield return _data[Math.Min(_stride - 1, Math.Max(0, i + 1))];
            yield return _data[Math.Min(_stride - 1, Math.Max(0, i + 1))];
        }

        public IEnumerable<T> SampleRegion(Rectangle2D region)
        {
            if (!_region.Overlaps(region))
                yield break;

            CellRegion r = RegionIndicesClamped(region);
            int i = r.Start;
            for (int y = 0; y <= r.Down; y++ )
                for (int x = 0; x <= r.Right; x++)
                    yield return _data[i + y * _stride + x];
        }

        public IEnumerable<T> SampleRegionOutward(Rectangle2D region)
        {
            if (!_region.Overlaps(region))
                yield break;

            CellRegion r = RegionIndicesClamped(region);
            //offset [ox, oy] starts at [0, 0] and spirals outward
            int ox = 0;
            int oy = 0;
            int dx = 0;
            int dy = -1;
            int halfX = r.Right / 2;
            int halfY = r.Down / 2;
            int tmp = Math.Max(r.Right, r.Down);
            int max = tmp * tmp;
            for (int i = 0; i < max; i++)
            {
                if((ox >= -halfX) && (ox <= halfX) && (oy >= -halfY) && (oy <= halfY))
                    yield return  _data[r.Start + halfX + ox + _stride * (halfY + oy)];
                if((ox == oy) || (ox < 0 && ox == -oy) || (ox > 0 && ox == 1-oy))
                {
                    tmp = dx;
                    dx = -dy;
                    dy = tmp;
                }
                ox += dx;
                oy += dy;
            }
        }

        public CellRegion RegionIndicesClamped(Rectangle2D rect)
        {
            rect = rect.Intersection(_region);
            rect.ExpandToGrid(_spacing);
            CellRegion region = new CellRegion();
            region.Start = CellIndexClamped(rect.TopLeft);
            float width = rect.Width / _spacing;
            region.Right = (int)width;
            if (region.Right <= width) //frac 0
                region.Right--;

            float height = rect.Height / _spacing;
            region.Down = (int)height;
            if (region.Down <= height) //frac 0
                region.Down--;
            return region;
        }

        public IEnumerable<T> SamplePerimeter(Rectangle2D rect)
        {
            CellRegion r = RegionIndicesClamped(rect);
            if (r.Right == 0 && r.Down == 0)
            {
                yield return _data[r.Start];
                yield break;
            }

            int tl = r.Start;
            int tr = r.Start + r.Right;
            int bl = tl + _stride * r.Down;
            int br = tr + _stride * r.Down;

            //TOPLEFT - > TOPRIGHT
            for (int i = tl; i < tr; i++)
                yield return _data[i];

            //TOPRIGHT -> BOTTOMRIGHT
            for (int i = tr; i < br; i += _stride)
                yield return _data[i];

            //BOTTOMRIGHT -> BOTTOMLEFT
            for (int i = br; i > bl; i--)
                yield return _data[i];

            //BOTTOMLEFT -> TOPLEFT
            for (int i = bl; i > tl; i -= _stride)
                yield return _data[i];

        }
        
        public T SampleOrDefault(Vector2D point)
        {
            if (!_innerRegion.Contains(point))
                return default(T);

            int index = CellIndex(point);
            return _data[index];
        }

        public Vector2D Constrain(Vector2D point)
        {
            return _innerRegion.Snapped(point);
        }

        public void Chart(Vector2D point, T newData)
        {
            int index = CellIndex(point);
            _data[index] = newData;
        }

        public Vector2D CellCenter(Vector2D point)
        {
            return _spacing * (CgMath.Floor(point * _toGrid) + Vector2D.Half);
        }

        public Vector2D NeighbourCellCenter(Vector2D pt)
        {
            Vector2D from = CellCenter(pt);
            Vector2D axis = (pt - from).DominantAxis;
            if (axis.IsAlmostZero)
                return from;
            axis.Normalize();
            return from + axis * Spacing;
        }

        public Vector2D CellTopLeft(Vector2D point)
        {
            return _spacing * CgMath.Floor(point * _toGrid);
        }

        public IEnumerable<Vector2D> CellsCenters
        {
            get
            {
                for (float x = _region.X; x < _region.Right; x += _spacing)
                    for (float y = _region.Y; y < _region.Bottom; y += _spacing)
                        yield return new Vector2D(x + 0.5f * _spacing, y + 0.5f * _spacing);
            }
        }

        public IEnumerable<Vector2D> CellsCentersInRegion(Rectangle2D rect)
        {
            Rectangle2D bounds = _region.Intersection(rect);
            bounds.ExpandToGrid(_spacing);

            for (float x = bounds.X; x < bounds.Right; x += _spacing)
                for (float y = bounds.Y; y < bounds.Bottom; y += _spacing)
                    yield return new Vector2D(x + 0.5f * _spacing, y + 0.5f * _spacing);
        }

        public IList<T> RawData
        {
            get { return _data; }
        }

        //PROTECTED

        protected struct Probe
        {
            public T TopLeft;
            public T TopRight;
            public T BottomLeft;
            public T BottomRight;
            public float U;
            public float V;
        }

        protected int CellIndex(Vector2D point)
        {
            point -= _region.TopLeft;
            point *= _toGrid;
            return (int)point.Y * _stride + (int)point.X;
        }

        protected int CellIndexClamped(Vector2D point)
        {
            point -= _region.TopLeft;
            point *= _toGrid;
            int x = Math.Min(_stride - 1, Math.Max(0, (int)point.X));
            int y = Math.Min(_rows - 1, Math.Max(0, (int)point.Y));
            return y * _stride + x;
        }

        protected void SampleProbe(Vector2D point, out Probe probe)
        {
            point = Constrain(point - Vector2D.Half * _spacing);
            Vector2D local = point - _region.TopLeft;
            Vector2D coord = CgMath.Floor(local / _spacing);
            int iY = (int)coord.Y * _stride;
            int iX = (int)coord.X;

            int iTopLeft = iY + iX;
            int iTopRight = iY + Math.Min(iX + 1, _stride - 1);
            int toBottom = (iTopLeft + _stride < _count) ? _stride : 0;

            Vector2D pTopLeft = CellTopLeft(point);
            Vector2D d = (point - pTopLeft) / _spacing;
            
            probe.TopLeft = _data[iTopLeft];
            probe.TopRight = _data[iTopRight];
            probe.BottomLeft = _data[iTopLeft + toBottom];
            probe.BottomRight = _data[iTopRight + toBottom];
            probe.U = d.X;
            probe.V = d.Y;
        }
    }
}
