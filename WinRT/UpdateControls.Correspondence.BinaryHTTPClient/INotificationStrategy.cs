using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public interface INotificationStrategy
    {
        event Action<FactTreeMemento> MessageReceived;
        IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid);
        Exception LastException { get; }
    }
}
