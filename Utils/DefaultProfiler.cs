using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics.Utils
{
    public class DefaultProfiler : IProfiler
    {
        class Accu
        {
            public Accu(string tag)
            {
                Tag = tag;
            }
            public bool Fresh;
            public string Tag;
            public double Last; //µs
            public double Peak; //µs
            public double Average; //µs
            public long Frames;
            public double PeakTreshold;
            public long StartTicks;
            public Accu Parent;
            public HashSet<Accu> Children = new HashSet<Accu>();
        }

        const long AVG_SAMPLE_COUNT = 100;

        private Stopwatch _watch = new Stopwatch();

        private Dictionary<string, Accu> _accus = new Dictionary<string, Accu>();
        private Accu _root = new Accu("ROOT");
        private Accu _current = null;

        public DefaultProfiler()
        {
            _current = _root;
        }

        public IEnumerable<ProfileRecord> Records
        {
            get
            {
                List<ProfileRecord> records = CollectRecords();
                return records;
            }
        }

        private List<ProfileRecord> CollectRecords()
        {
            List<ProfileRecord> records = new List<ProfileRecord>();
            foreach (var child in _root.Children)
                CollectRecords(child, records, 0);
            return records;
        }

        private void CollectRecords(Accu node, List<ProfileRecord> records, int depth)
        {
            records.Add(new ProfileRecord()
            {
                Depth = depth,
                Fresh = node.Fresh,
                Tag = node.Tag,
                Last = node.Last,
                Peak = node.Peak,
                Average = node.Average / Math.Min(node.Frames, AVG_SAMPLE_COUNT)
            });
            foreach (var child in node.Children)
                CollectRecords(child, records, depth + 1);
        }

        public void Push(string tag)
        {
            if (_current == _root)
            {
                _root.Children.Clear();
                _watch.Start();
            }

            string path = _current.Tag + "/" + tag;

            Accu ac = GetAccu(path);
            ac.Parent = _current;
            _current.Children.Add(ac);
            _current = ac;
            ac.Fresh = true;
            ac.StartTicks = _watch.ElapsedTicks;
        }

        private Accu GetAccu(string tag)
        {
            Accu ac = null;
            if (_accus.ContainsKey(tag))
                ac = _accus[tag];
            else
                ac = _accus[tag] = new Accu(tag);

            FlagStale(ac);
            return ac;
        }

        private void FlagStale(Accu accu)
        {
            accu.Fresh = false;
            foreach (var child in accu.Children)
                FlagStale(child);
        }

        public static string MicrosecsToString(int microsecs)
        {
            string s = microsecs.ToString("000000");
            return s.Insert(s.Length - 3, " ");
        }

        public double Pop()
        {
            double microSecs = ((_watch.ElapsedTicks - _current.StartTicks) / (double)Stopwatch.Frequency) * 1000000.0;

            _current.Frames++;
            _current.Last = microSecs;

            //PEAK
            if (microSecs > _current.PeakTreshold)
            {
                _current.Peak = microSecs;
                _current.PeakTreshold = 2f * microSecs;
            }
            _current.PeakTreshold *= 0.95f;

            //AVERAGE
            if (_current.Frames > AVG_SAMPLE_COUNT)
                _current.Average *= AVG_SAMPLE_COUNT / (float)(AVG_SAMPLE_COUNT + 1);
            _current.Average += microSecs;

            //move up the tree!
            _current = _current.Parent;
            if (_current == _root)
                _watch.Stop();

            return microSecs;
        }

        public void ResetAll()
        {
            _accus.Clear();
            _watch.Stop();
        }
    }
}
