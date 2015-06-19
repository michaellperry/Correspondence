using System;
using Correspondence.Mementos;
using Correspondence.Strategy;

namespace Correspondence.BinaryHTTPClient
{
    public interface INotificationStrategy
    {
        event Action<FactTreeMemento> MessageReceived;
        IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid);
        Exception LastException { get; }
    }
}
