using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface IMessageRepository
    {
        IEnumerable<FactID> LoadRecentMessages(FactID pivotId, TimestampID timestamp);
        FactMemento LoadFact(FactID factId);
        FactID SaveFact(FactMemento translatedMemento);
        bool FindExistingFact(FactMemento memento, out FactID id);
    }
}
