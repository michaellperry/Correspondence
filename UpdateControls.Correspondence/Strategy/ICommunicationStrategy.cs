using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        string ProtocolName { get; }
        string PeerName { get; }
        FactTreeMemento Get(FactTreeMemento rootTree, FactID rootId, TimestampID timestamp);
        void Post(FactTreeMemento messageBody);
    }
}
