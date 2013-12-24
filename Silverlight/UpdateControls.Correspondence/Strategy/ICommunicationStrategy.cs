using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        string ProtocolName { get; }
        string PeerName { get; }
        GetResultMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp);
        void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages);
    }
}
