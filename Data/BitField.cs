using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vectrics.Data
{
    public class BitField
    {
        private Dictionary<string, int> _flags = new Dictionary<string, int>();
        private int _next = 1;

        public BitField(params string[] names)
        {
            Add(names);
        }

        public BitField(Type type)
        {
            foreach(var name in Enum.GetNames(type))
            {
                int flagValue = (int)Enum.Parse(type, name);
                if(flagValue == _next)
                    EnsureFlag(name);
            }
        }

        public void Add(params string[] names)
        {
            foreach (var name in names)
                EnsureFlag(name);
        }

        public int GetBits(string name)
        {
            EnsureFlag(name);
            return _flags[name];
        }

        public int GetBits(params string[] names)
        {
            int result = 0;
            foreach (var name in names)
            {
                EnsureFlag(name);
                result |= _flags[name];
            }
            return result;
        }

        private void EnsureFlag(string name)
        {
            if (!_flags.ContainsKey(name))
            {
                _flags[name] = _next;
                _next *= 2;
            }
        }

        public IEnumerable<string> GetNames(int flags)
        {
            foreach (KeyValuePair<string, int> entry in _flags)
                if ((entry.Value & flags) != 0)
                    yield return entry.Key;
        }
    }
}
