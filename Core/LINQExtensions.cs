using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vectrics
{
    public static class LINQExtensions
    {
        public static TSource Best<TSource>(this IEnumerable<TSource> source, Func<TSource, float> cost)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (cost == null) throw new ArgumentNullException("rating");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                TSource best = sourceIterator.Current;
                float bestRating = cost(best);
                while (sourceIterator.MoveNext())
                {
                    float c = cost(sourceIterator.Current);
                    if (c < bestRating)
                    {
                        bestRating = c;
                        best = sourceIterator.Current;
                    }
                }
                return best;
            }
        }

        public static TSource BestOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, float> cost, TSource defaultValue = default(TSource))
        {
            if (source == null) throw new ArgumentNullException("source");
            if (cost == null) throw new ArgumentNullException("rating");
            TSource best = defaultValue;
            float bestRating = float.MaxValue;
            foreach (TSource element in source)
            {
                float c = cost(element);
                if (c < bestRating)
                {
                    bestRating = c;
                    best = element;
                }
            }
            return best;
        }

        public static IEnumerable<T> Lock<T>(this IEnumerable<T> source, object locker)
        {
            try
            {
                Monitor.Enter(locker);
                using (var e = source.GetEnumerator())
                    while (e.MoveNext())
                        yield return e.Current;
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }
    }
}
