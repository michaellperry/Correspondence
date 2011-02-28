using System;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
	public interface IAsynchronousCommunicationStrategy
	{
		string ProtocolName { get; }
		string PeerName { get; }
		void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Guid clientGuid, Action<FactTreeMemento, TimestampID> callback);
		void BeginPost(FactTreeMemento messageBody, Guid clientGuid, Action<bool> callback);

        event Action<FactTreeMemento> MessageReceived;
		IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid);
    }
}
