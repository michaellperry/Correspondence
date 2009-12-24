using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface IMessageRepository
    {
        TimestampID LoadTimestamp(string protocolName, string peerName);
        void SaveTimestamp(string protocolName, string peerName, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessages(ref TimestampID timestamp);
        FactMemento LoadFact(FactID factId);
    }
}
