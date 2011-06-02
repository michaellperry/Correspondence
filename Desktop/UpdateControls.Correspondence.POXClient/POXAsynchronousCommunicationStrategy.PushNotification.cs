using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;
using System;

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
            // Push notification is not supported in the desktop version.
            return new NoOpPushSubscription();
        }

        public bool IsLongPolling
        {
            get { return _configuration.TimeoutSeconds > 0; }
        }
    }
}