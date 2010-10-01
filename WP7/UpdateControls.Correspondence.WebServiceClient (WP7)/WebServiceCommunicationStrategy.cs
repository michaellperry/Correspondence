using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get
            {
                return "http://localhost:9119/SyncExpress";
                //return new SynchronizationServiceClient().Endpoint.Address.ToString();
            }
        }

        public void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Action<FactTreeMemento> callback)
        {
            FactTree pivot = Translate.MementoToFactTree(pivotTree);
            ISynchronizationService synchronizationService = new SynchronizationServiceClient();
            synchronizationService.BeginGet(pivot, pivotId.key, timestamp.Key, delegate(IAsyncResult result)
            {
                FactTree factTree = synchronizationService.EndGet(result);
                callback(Translate.FactTreeToMemento(factTree));
            }, null);
        }

        public void BeginPost(FactTreeMemento messageBody, Action callback)
        {
            ISynchronizationService synchronizationService = new SynchronizationServiceClient();
            synchronizationService.BeginPost(Translate.MementoToFactTree(messageBody), delegate(IAsyncResult result)
            {
                synchronizationService.EndPost(result);
                callback();
            }, null);
        }


        public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Action<FactTreeMemento> callback)
        {
            throw new NotImplementedException();
        }
    }
}
