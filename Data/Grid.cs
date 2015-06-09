using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics
{
    public class Grid<T>
    {
        public class DataPoint
        {
            public Vector2D Point;
            public T Data;
        }

        private float _spacing = 0.25f;
        private List<T> _data = null;
        private Rectangle2D _region;
        private int _stride;
        private int _count;

        public float Spacing
        {
            get { return _spacing; }
        }

        public Rectangle2D Region
        {
            get { return _region; }
        }

        public Grid(Rectangle2D region, float spacing) //resolution in cells per meter
        {
            _spacing = spacing;
            _region = region;
            _region.ExpandToGrid(_spacing);
            Vector2D topLeft = CellsCenters.First();
            _stride = CellsCenters.Where(cc => cc.Y == topLeft.Y).Count();
            _count = CellsCenters.Count();
            _data = Enumerable.Repeat(default(T), _count).ToList();
        }

        public T Sample(Vector2D point)
        {
            int index = ToNearestCellIndex(point);
            return _data[index];
        }

        public void Chart(Vector2D point, T newData)
        {
            int index = ToNearestCellIndex(point);
            _data[index] = newData;
        }

        private int ToNearestCellIndex(Vector2D point)
        {
            point = _region.Snapped(point);
            Vector2D coord = CgMath.Floor((point - _region.TopLeft) / _spacing);
            return (int)coord.Y * _stride + (int)coord.X;
        }

        public Vector2D ToNearestCellCenter(Vector2D point)
        {
            return _spacing * (CgMath.Round((point / _spacing) + Vector2D.Half) - Vector2D.Half);
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
    }
}
