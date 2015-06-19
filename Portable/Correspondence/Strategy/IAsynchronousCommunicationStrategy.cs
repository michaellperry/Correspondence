using System;
using Correspondence.Mementos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Correspondence.Strategy
{
	public interface IAsynchronousCommunicationStrategy
	{
        string ProtocolName { get; }
		string PeerName { get; }
        Task<GetManyResultMemento> GetManyAsync(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid);
        Task PostAsync(FactTreeMemento messageBody, Guid clientGuid, List<UnpublishMemento> unpublishedMessages);
        Task InterruptAsync(Guid clientGuid);
        Task NotifyAsync(FactTreeMemento messageBody, FactID pivotId, Guid clientGuid, string text1, string text2);
        bool IsLongPolling { get; }

        event Action<FactTreeMemento> MessageReceived;
		Task<IPushSubscription> SubscribeForPushAsync(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid);
        Exception LastException { get; }
    }
}
