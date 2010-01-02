using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface IMessageRepository
    {
        T FindFact<T>(T prototype)
            where T : CorrespondenceFact;
        T AddFact<T>(T prototype) where T : CorrespondenceFact;
        TimestampID LoadOutgoingTimestamp(string protocolName, string peerName);
        void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp);
        TimestampID LoadIncomingTimestamp(string protocolName, string peerName, CorrespondenceFact pivot);
        void SaveIncomingTimestamp(string protocolName, string peerName, CorrespondenceFact pivot, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessages(TimestampID timestamp);
        IEnumerable<FactID> LoadRecentMessages(FactID pivotId, TimestampID timestamp);
        FactMemento LoadFact(FactID factId);
        CorrespondenceFact GetFactByID(FactID factId);
        FactID IDOfFact(CorrespondenceFact fact);
        FactID SaveFact(FactMemento translatedMemento);
        bool FindExistingFact(FactMemento memento, out FactID id);
    }
}
