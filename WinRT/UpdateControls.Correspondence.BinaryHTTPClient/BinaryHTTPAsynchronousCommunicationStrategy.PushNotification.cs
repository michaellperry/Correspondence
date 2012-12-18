using System;
using System.Threading.Tasks;
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
        private INotificationStrategy _notificationStrategy;

        public BinaryHTTPAsynchronousCommunicationStrategy SetNotificationStrategy(INotificationStrategy notificationStrategy)
        {
            _notificationStrategy = notificationStrategy;
            _notificationStrategy.MessageReceived += OnMessageReceived;
            return this;
        }

        partial void Initialize()
        {
        }

        public bool IsLongPolling
        {
            get { return true; }
        }

        public Task<IPushSubscription> SubscribeForPushAsync(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            if (_notificationStrategy != null)
                return Task.FromResult(_notificationStrategy.SubscribeForPush(pivotTree, pivotId, clientGuid));
            else
                return Task.FromResult((IPushSubscription)new NoOpPushSubscription());
        }

        private void OnMessageReceived(FactTreeMemento factTree)
        {
            if (MessageReceived != null)
                MessageReceived(factTree);
        }
    }
}