using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectrics.Data
{
    public class Watcher
    {
        public delegate T Monitor<T>();
        public delegate void Handler<T>(T oldValue, T newValue);
        public interface IWatchTarget
        {
            void RemoveLater();
            bool IsToBeRemoved();
            void Evaluate();
        }
        public class WatchTarget<T> : IWatchTarget
        {
            bool ToBeRemoved;
            public bool IsToBeRemoved() { return ToBeRemoved; }
            public void RemoveLater() { ToBeRemoved = true; }
            public T State;
            public Monitor<T> Eval;
            public Handler<T> Handler;
            public void Evaluate()
            {
                T currentState = Eval();
                if(!EqualityComparer<T>.Default.Equals(State, currentState))
                {
                    T oldState = State;
                    State = currentState;
                    Handler(oldState, currentState);
                }
            }
        }

        bool _updating = false;
        List<IWatchTarget> _targets = new List<IWatchTarget>();

        public IWatchTarget Watch<T>(Monitor<T> monitor, Handler<T> onChange)   //adds a monitor to the list of observees
        {
            WatchTarget<T> observee = new WatchTarget<T>()
            {
                State = monitor(),
                Eval = monitor,
                Handler = onChange
            };
            _targets.Add(observee);
            return observee;
        }

        public void Update()
        {
            _updating = true;
            foreach(var target in _targets)
                target.Evaluate();

            _targets.RemoveAll(w => w.IsToBeRemoved());
            _updating = false;
        }

        public void Remove(IWatchTarget target, bool evalFirst = true)
        {
            if (_updating)
                target.RemoveLater();
            else
            {
                if (evalFirst)
                {
                    _updating = true;
                    target.Evaluate();
                    _updating = false;
                }
                _targets.Remove(target);
            }
        }
    }
}
