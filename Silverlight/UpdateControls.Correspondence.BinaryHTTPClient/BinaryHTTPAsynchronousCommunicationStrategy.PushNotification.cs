using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public class NoOpPushSubscription : IPushSubscription
    {
        public void Unsubscribe()
        {
        }
    }

    public partial class BinaryHTTPAsynchronousCommunicationStrategy
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