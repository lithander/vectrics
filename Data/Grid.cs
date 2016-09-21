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

        public void Clear(Func<T> generator)
        {
            int max_i = _data.Count;
            for (int i = 0; i < max_i; i++)
                _data[i] = generator();
        }

        public T Sample(Vector2D point)
        {
            int index = CellIndexClamped(point);
            return _data[index];
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

        public IEnumerable<T> SamplePerimeter(Rectangle2D rect)
        {
            if (!_region.Overlaps(rect))
                yield break;

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

        public IEnumerable<T> SampleOutward(Vector2D point, float range)
        {
            //this iterates over all cells within 'range' of 'point' in a pattern so that 
            //for each visited cell any neighbours closer to 'point' have allready been visited
            point -= _region.TopLeft;
            point *= _toGrid;
            int py = (int)point.Y;
            int px = (int)point.X;
            if (px < _stride && px >= 0 && py < _rows && py >= 0)
                yield return _data[px + _stride * py];
            int max_i = (int)Math.Ceiling(range * Math.Sqrt(2));
            for (int i = 1; i <= max_i; i++)
                for (int j = (int)Math.Max(0, i - range); j < i && j <= range; j++)
                {
                    int x = px - i + j;
                    int y = py - j;
                    if (x < _stride && x >= 0 && y < _rows && y >= 0)
                        yield return _data[x + _stride * y];
                    x = px + i - j;
                    y = py + j; 
                    if (x < _stride && x >= 0 && y < _rows && y >= 0)
                        yield return _data[x + _stride * y];
                    x = px + j;
                    y = py - i + j;
                    if (x < _stride && x >= 0 && y < _rows && y >= 0)
                        yield return _data[x + _stride * y];
                    x = px - j;
                    y = py + i - j;
                    if (x < _stride && x >= 0 && y < _rows && y >= 0)
                        yield return _data[x + _stride * y];
                }
        }


        public IEnumerable<T> SampleRegionOutward(Rectangle2D region)
        {
            if (!_region.Overlaps(region))
                yield break;

            CellRegion r = RegionIndicesClamped(region);
            if (r.Right == 0 && r.Down == 0)
            {
                yield return _data[r.Start];
                yield break;
            }

            int c = CellIndexClamped(region.Center); //cell where the iteration starts
            int px = c % _stride; //grid column of the start cell
            int py = c / _stride; //grid row of the start cell
            //cells outside these constrains need to be skipped
            int x0 = r.Start % _stride;
            int y0 = r.Start / _stride;
            int x1 = x0 + r.Right;
            int y1 = y0 + r.Down;
            //variables to track current offset and direction
            int ox = 0;
            int oy = 0;
            int dx = 0;
            int dy = -1;
            int remaining = (1+r.Right) * (1+r.Down); //the expected amount of samples so we know when to stop
            while(remaining > 0)
            {
                int x = px + ox;
                int y = py + oy;
                //within clamped region?
                if(x >= x0 && x <= x1 && y >= y0 && y <= y1)
                {
                    yield return _data[x + _stride * y];
                    remaining--;
                }
                //change direction?
                if ((ox == oy) || (ox < 0 && ox == -oy) || (ox > 0 && ox == 1 - oy))
                {
                    int tmp = dx;
                    dx = -dy;
                    dy = tmp;
                }
                //next
                ox += dx;
                oy += dy;
            }
        }

        //Von Neumann neighborhood
        public IEnumerable<T> Sample4Connected(Vector2D point)
        {
            int c = CellIndexClamped(point);
            int x = c % _stride; //grid column of the start cell
            int y = c / _stride; //grid row of the start cell
            if(x + 1 < _stride)
                yield return _data[(x + 1) + _stride * y];
            if (y + 1 < _rows)
                yield return _data[x + _stride * (y+1)];
            if (x > 0)
                yield return _data[(x - 1) + _stride * y];
            if (y > 0)
                yield return _data[x + _stride * (y - 1)];
        }

        //Moore neighborhood
        public IEnumerable<T> Sample8Connected(Vector2D point)
        {
            int c = CellIndexClamped(point);
            int x = c % _stride; //grid column of the start cell
            int y = c / _stride; //grid row of the start cell
            if (x + 1 < _stride)
                yield return _data[(x + 1) + _stride * y];
            if (y + 1 < _rows)
                yield return _data[x + _stride * (y + 1)];
            if (x > 0)
                yield return _data[(x - 1) + _stride * y];
            if (y > 0)
                yield return _data[x + _stride * (y - 1)];
            //like von Neuman but now adde the diagonals
            if (x + 1 < _stride && y > 0)
                yield return _data[(x + 1) + _stride * (y - 1)];
            if (x + 1 < _stride && y + 1 < _rows)
                yield return _data[(x + 1) + _stride * (y + 1)];
            if (x > 0 && y + 1 < _rows)
                yield return _data[(x - 1) + _stride * (y + 1)];
            if (x > 0 && y > 0)
                yield return _data[(x - 1) + _stride * (y - 1)];
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

        public void ChartRaw(Vector2D point, T newData)
        {
            int index = CellIndex(point);
            _data[index] = newData;
        }

        public void Chart(Vector2D point, T newData)
        {
            int index = CellIndexClamped(point);
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
        
        public CellRegion RegionIndicesClamped(Rectangle2D rect)
        {
            rect = rect.Intersection(_region);
            CellRegion region = new CellRegion();

            //.0 is considered the start of a new cell
            Vector2D p1 = _toGrid * (rect.TopLeft - _region.TopLeft);
            int x1 = Math.Min(_stride - 1, Math.Max(0, (int)p1.X));
            int y1 = Math.Min(_rows - 1, Math.Max(0, (int)p1.Y));
            region.Start = y1 * _stride + x1;

            //.0 is included in the previous cell
            Vector2D p2 = _toGrid * (rect.BottomRight - _region.TopLeft);
            int x2 = Math.Min(_stride - 1, Math.Max(x1, (int)(Math.Ceiling(p2.X) - 1)));
            int y2 = Math.Min(_rows - 1, Math.Max(y1, (int)(Math.Ceiling(p2.Y) - 1)));
            region.Right = x2 - x1;
            region.Down = y2 - y1;

            return region;
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
