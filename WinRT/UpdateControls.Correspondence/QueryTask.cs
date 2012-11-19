using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence
{
    public class QueryTask
    {
        private bool _completedSynchronously;
        private List<CorrespondenceFact> _result;
        private Action<QueryTask> _continuation = null;

        public QueryTask() :
            this(false, null)
        {
        }

        private QueryTask(bool completedSynchronously, List<CorrespondenceFact> result)
        {
            _completedSynchronously = completedSynchronously;
            _result = result;
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
        }

        public List<CorrespondenceFact> Result
        {
            get { return _result; }
        }

        public void ContinueWith(Action<QueryTask> continuation)
        {
            if (!SetContinuation(continuation))
                continuation(this);
        }

        private bool SetContinuation(Action<QueryTask> continuation)
        {
            lock (this)
            {
                if (_result == null)
                {
                    _continuation = continuation;
                    return true;
                }
            }
            return false;
        }

        public void Complete(List<CorrespondenceFact> result)
        {
            Action<QueryTask> continuation = SetResult(result);
            if (continuation != null)
                continuation(this);
        }

        private Action<QueryTask> SetResult(List<CorrespondenceFact> result)
        {
            lock (this)
            {
                _result = result;
                var continuation = _continuation;
                _continuation = null;
                return continuation;
            }
        }

        public static QueryTask FromResult(List<CorrespondenceFact> result)
        {
            return new QueryTask(true, result);
        }
    }
}
