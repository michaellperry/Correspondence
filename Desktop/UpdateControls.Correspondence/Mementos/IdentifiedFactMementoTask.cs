using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Mementos
{
    public class CompletedIdentifiedFactMementoTask : IdentifiedFactMementoTask
    {
        private List<IdentifiedFactMemento> _result;

        public override bool CompletedSynchronously
        {
            get { return true; }
        }

        public override List<IdentifiedFactMemento> Result
        {
            get { return _result; }
        }

        public static IdentifiedFactMementoTask FromResult(List<IdentifiedFactMemento> result)
        {
            var task = new CompletedIdentifiedFactMementoTask();
            task._result = result;
            return task;
        }
    }

    public abstract class IdentifiedFactMementoTask
    {
        public abstract bool CompletedSynchronously { get; }
        public abstract List<IdentifiedFactMemento> Result { get; }
    }
}
