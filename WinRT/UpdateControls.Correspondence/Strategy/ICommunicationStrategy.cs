using System.Collections.Generic;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
    public interface ICommunicationStrategy
    {
        string ProtocolName { get; }
        string PeerName { get; }
        Task<GetResultMemento> GetAsync(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp);
        void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages);
    }
}
