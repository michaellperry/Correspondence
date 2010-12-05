using UpdateControls.Correspondence.Mementos;
using System;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        string ProtocolName { get; }
        string PeerName { get; }
        GetResultMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp);
        void Post(FactTreeMemento messageBody);
    }
}
