using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vectrics.Utils
{
    public struct ProfileRecord
    {
        public bool Fresh;
        public int Depth;
        public string Tag;
        public double Last; //µs
        public double Peak; //µs
        public double Average; //µs
    }

    public interface IProfiler
    {
        double Pop();
        void Push(string tag);
        IEnumerable<ProfileRecord> Records { get; }
        void ResetAll();
    }

    public struct LoadRecord
    {
        public string Tag;
        public double Last; //0..1
        public double Peak; //0..1
        public double Average; //0..1
    }

    public interface ILoadMonitor
    {
        void StartUse(string tag);
        void StopUse(string tag);
        void Watch(string tag);
        void Unwatch(string tag);        
        IEnumerable<LoadRecord> Records { get; }
        void ResetAll();
    }

    public static class Profiler
    {
        private static IProfiler _profiler = new DefaultProfiler();
        private static ILoadMonitor _loadMonitor = new DefaultLoadMonitor();
        public static Action<string> OnPush;
        public static Action OnPop;
        public static Thread Filter = null;
        public static bool Enabled = false;
        
        public static void SetProfiler(IProfiler profiler)
        {
            _profiler = profiler;
        }

        public static void SetLoadMonitor(ILoadMonitor loadMon)
        {
            _loadMonitor = loadMon;
        }

        public static double Pop()
        {
            if (!Enabled) return -1;
            if (Filter != null && Filter != Thread.CurrentThread)
                return -1;

            if (OnPop != null)
                OnPop();

            return _profiler.Pop();
        }
        public static void Push(string tag)
        {
            if (!Enabled) return;
            if (Filter != null && Filter != Thread.CurrentThread)
                return;

            if (OnPush != null)
                OnPush(tag);

            _profiler.Push(tag);
        }
        public static IEnumerable<ProfileRecord> TimeRecords 
        { 
            get { return _profiler.Records; }
        }

        public static void StartUse(string tag)
        {
            if (!Enabled) return;
            if (Filter != null && Filter != Thread.CurrentThread)
                return;

            _loadMonitor.StartUse(tag);
        }
        public static void StopUse(string tag)
        {
            if (!Enabled) return;
            if (Filter != null && Filter != Thread.CurrentThread)
                return;

            _loadMonitor.StopUse(tag);
        }
        public static void Watch(string tag)
        {
            if (!Enabled) return;
            if (Filter != null && Filter != Thread.CurrentThread)
                return;

            _loadMonitor.Watch(tag);
        }
        public static void Unwatch(string tag)
        {
            if (!Enabled) return;
            if (Filter != null && Filter != Thread.CurrentThread)
                return;

            _loadMonitor.Unwatch(tag);
        }

        public static IEnumerable<LoadRecord> LoadRecords
        {
            get { return _loadMonitor.Records; }
        }

        public static void ResetAll()
        {
            _profiler.ResetAll();
        }
    }    
}
