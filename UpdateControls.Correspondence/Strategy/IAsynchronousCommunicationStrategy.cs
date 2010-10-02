using System;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Strategy
{
	public interface IAsynchronousCommunicationStrategy
	{
		string ProtocolName { get; }
		string PeerName { get; }
		void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Action<FactTreeMemento> callback);
		void BeginPost(FactTreeMemento messageBody, Action callback);

        event Action<FactTreeMemento> MessageReceived;
        IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId);
    }
}
