using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Mementos
{
    public class IdentifiedFactMementoTask
    {
        private bool _completedSynchronously = true;
        private List<IdentifiedFactMemento> _result;

        private IdentifiedFactMementoTask()
        {
        }

        public bool CompletedSynchronously
        {
            get { return _completedSynchronously; }
        }

        public List<IdentifiedFactMemento> Result
        {
            get { return _result; }
        }

        public static IdentifiedFactMementoTask FromResult(List<IdentifiedFactMemento> result)
        {
            var task = new IdentifiedFactMementoTask();
            task._result = result;
            return task;
        }
    }
}
