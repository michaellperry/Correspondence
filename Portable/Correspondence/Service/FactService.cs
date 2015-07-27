using Assisticant.Fields;
using Correspondence.Threading;
using System;
using System.Threading.Tasks;

namespace Correspondence.Service
{
    class FactService<TFact> : Process
        where TFact : CorrespondenceFact
    {
        private readonly Computed<TFact> _nextWorkItem;
        private ComputedSubscription _subscription;

        public FactService(Func<TFact> nextWorkItem, Func<TFact, Task> processAsync)
        {
            _nextWorkItem = new Computed<TFact>(() => nextWorkItem());
            _subscription = _nextWorkItem.Subscribe(delegate(TFact workItem)
            {
                if (workItem != null)
                    Perform(() => processAsync(workItem));
            });
        }
    }
}
