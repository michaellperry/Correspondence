using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Tasks;

namespace UpdateControls.Correspondence.Memory
{
    public interface IFuture
    {
        void CalculateResults();
        void DeliverResults();
    }

    public class Future<TResult> : IFuture
    {
        private Func<TResult> _action;
        private TResult _result;
        private List<Action<Future<TResult>>> _continuations = new List<Action<Future<TResult>>>();

        public Future(Func<TResult> action)
        {
            _action = action;
        }

        public void CalculateResults()
        {
            _result = _action();
        }

        public void DeliverResults()
        {
            foreach (var continuation in _continuations)
                continuation(this);
        }

        public TResult Result
        {
            get
            {
                if (_result == null)
                    throw new InvalidOperationException("Checking the result before the task is done.");

                return _result;
            }
        }

        public void ContinueWith(Action<Future<TResult>> continuation)
        {
            _continuations.Add(continuation);
        }
    }
}
