using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics.Utils
{
    public class DefaultLoadMonitor : ILoadMonitor
    {
        const long AVG_SAMPLE_COUNT = 100;

        private Stopwatch _watch = new Stopwatch();
        private Dictionary<string, UsageToken> _tokens = new Dictionary<string, UsageToken>();

        public IEnumerable<LoadRecord> Records
        {
            get
            {
                foreach (var entry in _tokens)
                {
                    LoadRecord rec = new LoadRecord();
                    var token = entry.Value;
                    rec.Average = token.AverageSum / Math.Min(token.Frames, AVG_SAMPLE_COUNT);
                    rec.Last = token.Last;
                    rec.Peak = token.Peak;
                    rec.Tag = entry.Key;
                    yield return rec;
                }
            }
        }

        class UsageToken
        {
            private Stopwatch _watch;

            public UsageToken(Stopwatch stopwatch)
            {
                _watch = stopwatch;
                LastTicks = _watch.ElapsedTicks;
                _count = 0;
            }

            private int _count = 0;
            public int UseCount
            {
                get
                {
                    return _count;
                }

                set
                {
                    bool prevUsed = _count > 0;
                    long ticks = _watch.ElapsedTicks;
                    if (prevUsed)
                        UsedTicks += ticks - LastTicks;
                    else
                        UnusedTicks += ticks - LastTicks;

                    LastTicks = ticks;
                    _count = Math.Max(0, value);
                }
            }
            public long LastTicks;
            public long UsedTicks;
            public long UnusedTicks;
            public bool Active;

            public double Last; //0..1
            public double Peak; //0..1
            public double AverageSum; //0..1
            public double PeakTreshold;
            public int Frames;

            internal void Start()
            {
                Active = true;
                LastTicks = _watch.ElapsedTicks;
                UsedTicks = 0;
                UnusedTicks = 0;
                Frames++;
            }

            internal void Stop()
            {
                Log.Assert(UseCount == 0);
                UnusedTicks += _watch.ElapsedTicks - LastTicks;

                Last = UsedTicks / (double)(UsedTicks + UnusedTicks);
                //PEAK
                if (Last > PeakTreshold)
                {
                    Peak = Last;
                    PeakTreshold = 2f * Last;
                }
                PeakTreshold *= 0.95f;

                //AVERAGE
                if (Frames > AVG_SAMPLE_COUNT)
                    AverageSum *= AVG_SAMPLE_COUNT / (float)(AVG_SAMPLE_COUNT + 1);
                AverageSum += Last;
            }
        }

        public DefaultLoadMonitor()
        {
            _watch.Start();
        }

        private void EnsureTag(string tag)
        {
            if (!_tokens.ContainsKey(tag))
                _tokens[tag] = new UsageToken(_watch);
        }

        public void StartUse(string tag)
        {
            EnsureTag(tag);
            Log.Assert(_tokens[tag].Active, "Tried to increase use counter of unwatched resource " + tag + "!");
            _tokens[tag].UseCount++;
        }
         
        public void StopUse(string tag)
        {
            EnsureTag(tag);
            Log.Assert(_tokens[tag].Active, "Tried to decrease use counter of unwatched resource " + tag + "!");
            _tokens[tag].UseCount--;
        }

        public void Watch(string tag)
        {
            EnsureTag(tag);
            //Debug.Assert(!_tokens[tag].Active, "Watching load on " + tag + " was allready started!");
            var token = _tokens[tag];
            if (token.Active)
                token.Stop();
            token.Start();
        }

        public void Unwatch(string tag)
        {
            EnsureTag(tag);
            Log.Assert(_tokens[tag].Active, "Watching load on " + tag + " wasn't started!");
            Log.Assert(_tokens[tag].UseCount == 0, "Can't stop watching when use count isn't ZERO on " + tag);
            _tokens[tag].Stop();
        }

        public void ResetAll()
        {
            _watch.Reset();
            _watch.Start();
            _tokens.Clear();
        }
    }
}
