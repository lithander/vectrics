using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Vectrics
{
    public class RingList<T> : IList<T>
    {
        IList<T> _wrapped;

        public RingList(IList<T> data)
        {
            _wrapped = data;
        }

        public int IndexOf(T item)
        {
            return _wrapped.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            int idx = index % (_wrapped.Count);
            _wrapped.Insert(idx, item);
        }

        public void RemoveAt(int index)
        {
            int idx = index % (_wrapped.Count);
            _wrapped.RemoveAt(idx);
        }

        public T this[int index]
        {
            get
            {
                int idx = index % (_wrapped.Count);
                return _wrapped[idx];
            }
            set
            {
                int idx = index % (_wrapped.Count);
                _wrapped[idx] = value;
            }
        }

        public void Add(T item)
        {
            _wrapped.Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(T item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _wrapped.Count; }
        }

        public bool IsReadOnly
        {
            get { return _wrapped.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return _wrapped.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }
    }

    /*
	public class VectorList
    {
		protected Vector2D[] m_VertexArray;
		
		public VectorList(int count)
        {
            m_VertexArray = new Vector2D[count];
        }

        public VectorList(params Vector2D[] vertices)
        {
            int i = 0;
            m_VertexArray = new Vector2D[vertices.Length];
            foreach (Vector2D v in vertices)
            {
                m_VertexArray[i++] = v;
            }
        }

        public int Count
        {
            get { return m_VertexArray.Length; }
        }

        public Vector2D this[int i]
        {
            get
            {
                int idx = i % (m_VertexArray.Length);
                return m_VertexArray[idx];
            }
            set
            {
                int idx = i % (m_VertexArray.Length);
                m_VertexArray[idx] = value;
            }
        }
        */
}
