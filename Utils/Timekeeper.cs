using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics.Utils
{
    public class Timekeeper
    {
        public struct Record
        {
            public string Tag;
            public double Peak; //µs
            public double Average; //µs
        }

        class Accu
        {
            public double Peak; //µs
            public double Average; //µs
            public double PeakTreshold;
            public long StartTicks;
        }

        private Dictionary<string, Accu> _accus = new Dictionary<string, Accu>();
        private Dictionary<string, Accu> _open = new Dictionary<string, Accu>();
        private Stopwatch _watch = new Stopwatch();

        public IEnumerable<Record> Records
        {
            get
            {
                return _accus.Select(kv => new Record()
                {
                    Tag = kv.Key,
                    Peak = kv.Value.Peak,
                    Average = kv.Value.Average / 100.0f
                });
            }
        }

        public void Begin(string tag)
        {
            Accu ac = null;
            if (_accus.ContainsKey(tag))
                ac = _accus[tag];
            else
            {
                ac = new Accu();
                _accus[tag] = ac;
            }

            if (_open.Count == 0)
                _watch.Start();

            ac.StartTicks = _watch.ElapsedTicks;
            _open[tag] = ac;
        }

        public void End(string tag)
        {
            Accu ac = _open[tag];
            double microSec = ((_watch.ElapsedTicks - ac.StartTicks) / (double)Stopwatch.Frequency) * 1000000.0;

            if (microSec > ac.PeakTreshold)
            {
                ac.Peak = microSec;
                ac.PeakTreshold = 2f * microSec;
            }
            ac.PeakTreshold *= 0.95f;

            ac.Average *= 0.99f;
            ac.Average += microSec;

            _open.Remove(tag);
            if (_open.Count == 0)
                _watch.Stop();
        }

        public void Reset(string tag)
        {
            _open.Remove(tag);
            _accus.Remove(tag);
        }

        public void ResetAll()
        {
            _open.Clear();
            _accus.Clear();
            _watch.Stop();
        }
    }
}
