using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence
{
    public class QueryTask
    {
        private bool _completedSynchronously = false;
        private List<CorrespondenceFact> _result;
        private List<Action<QueryTask>> _continuations = new List<Action<QueryTask>>();

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
            _continuations.Add(continuation);
        }

        public void Complete(List<CorrespondenceFact> result)
        {
            _result = result;
            foreach (var continuation in _continuations)
                continuation(this);
        }

        public static QueryTask FromResult(List<CorrespondenceFact> result)
        {
            var task = new QueryTask();
            task._completedSynchronously = true;
            task._result = result;
            return task;
        }
    }
}
