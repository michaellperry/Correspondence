using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Memory
{
    public class FutureIdentifiedFactMementoTask : IdentifiedFactMementoTask
    {
        private Func<List<IdentifiedFactMemento>> _action;
        private List<IdentifiedFactMemento> _result;

        public FutureIdentifiedFactMementoTask(Func<List<IdentifiedFactMemento>> action)
        {
            _action = action;
        }

        public void Run()
        {
            _result = _action();
        }

        public override bool CompletedSynchronously
        {
            get { return false; }
        }

        public override List<IdentifiedFactMemento> Result
        {
            get
            {
                if (_result == null)
                    throw new InvalidOperationException("Checking the result before the task is done.");

                return _result;
            }
        }
    }
}
