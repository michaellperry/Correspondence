using System;
using Correspondence.Mementos;
using System.Collections.Generic;

namespace Correspondence.Strategy
{
	public interface IAsynchronousCommunicationStrategy
	{
        string ProtocolName { get; }
		string PeerName { get; }
        void BeginGetMany(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid, Action<FactTreeMemento, IEnumerable<PivotMemento>> callback, Action<Exception> error);
        void BeginPost(FactTreeMemento messageBody, Guid clientGuid, List<UnpublishMemento> unpublishedMessages, Action<bool> callback, Action<Exception> error);
        void Interrupt(Guid clientGuid);
        void Notify(FactTreeMemento messageBody, FactID pivotId, Guid clientGuid, string text1, string text2);
        bool IsLongPolling { get; }

        event Action<FactTreeMemento> MessageReceived;
		IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid);
    }
}
