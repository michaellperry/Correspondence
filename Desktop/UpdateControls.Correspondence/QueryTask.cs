using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence
{
    public class QueryTask
    {
        private bool _completedSynchronously = true;
        private List<CorrespondenceFact> _result;

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
            throw new NotImplementedException();
        }

        public static QueryTask FromResult(List<CorrespondenceFact> result)
        {
            var task = new QueryTask();
            task._result = result;
            return task;
        }
    }
}
