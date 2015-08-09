using Assisticant.Fields;
using Correspondence.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Correspondence.Service
{
    class FactService<TFact> : Process
        where TFact : CorrespondenceFact
    {
        private readonly Computed<IEnumerable<TFact>> _nextWorkItem;
        private ComputedSubscription _subscription;
        private HashSet<TFact> _processed = new HashSet<TFact>();

        public FactService(Func<IEnumerable<TFact>> nextWorkItem, Func<TFact, Task> processAsync)
        {
            _nextWorkItem = new Computed<IEnumerable<TFact>>(() => nextWorkItem());
            _subscription = _nextWorkItem.Subscribe(delegate(IEnumerable<TFact> workItems)
            {
                if (workItems != null)
                {
                    foreach (var workItem in workItems)
                    {
                        if (workItem != null && !_processed.Contains(workItem))
                        {
                            Perform(() => processAsync(workItem));
                            _processed.Add(workItem);
                        }
                    }
                }
            });
        }
    }
}
