using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Correspondence.Tasks
{
    public class Task<TResult>
    {
        private bool _completedSynchronously;
        private bool _completed;
        private TResult _result;
        private List<Action<Task<TResult>>> _continuations;

        public Task() :
            this(false, default(TResult))
        {
        }

        private Task(bool completedSynchronously, TResult result)
        {
            _completedSynchronously = completedSynchronously;
            _completed = completedSynchronously;
            _result = result;
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
        }

        public TResult Result
        {
            get
            {
                lock (this)
                {
                    if (!_completed)
                        throw new InvalidOperationException("Observing the result of a task before it is completed.");

                    return _result;
                }
            }
        }

        public void ContinueWith(Action<Task<TResult>> continuation)
        {
            if (!AddContinuation(continuation))
                continuation(this);
        }

        public Task<T> ContinueWith<T>(Func<Task<TResult>, T> continuation)
        {
            Task<T> task = new Task<T>();
            if (AddContinuation(new Action<Task<TResult>>(delegate(Task<TResult> r)
                {
                    T t = continuation(r);
                    task.Complete(t);
                })))
                return task;

            return Task<T>.FromResult(continuation(this));
        }

        private bool AddContinuation(Action<Task<TResult>> continuation)
        {
            lock (this)
            {
                if (!_completed)
                {
                    if (_continuations == null)
                        _continuations = new List<Action<Task<TResult>>>();
                    _continuations.Add(continuation);
                    return true;
                }
            }
            return false;
        }

        public void Complete(TResult result)
        {
            List<Action<Task<TResult>>> continuations = SetResult(result);
            if (continuations != null)
                continuations.ForEach(c => c(this));
        }

        private List<Action<Task<TResult>>> SetResult(TResult result)
        {
            lock (this)
            {
                _result = result;
                _completed = true;
                var continuations = _continuations;
                _continuations = null;
                return continuations;
            }
        }

        public static Task<TResult> FromResult(TResult result)
        {
            return new Task<TResult>(completedSynchronously: true, result: result);
        }
    }
}
