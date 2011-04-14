using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.POXClient
{
    public class NoOpPushSubscription : IPushSubscription
    {
        public void Unsubscribe()
        {
        }
    }

    public partial class POXAsynchronousCommunicationStrategy
    {
        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            // Push notification is not supported in the silverlight version (yet).
            return new NoOpPushSubscription();
        }
    }
}