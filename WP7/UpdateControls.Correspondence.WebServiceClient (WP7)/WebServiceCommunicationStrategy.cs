using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
        private SynchronizationServiceClient _synchronizationServiceClient;
        private ISynchronizationService _synchronizationService;

        public WebServiceCommunicationStrategy()
        {
            _synchronizationServiceClient = new SynchronizationServiceClient();
            _synchronizationService = _synchronizationServiceClient;
        }

        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get { return _synchronizationServiceClient.Endpoint.Address.ToString(); }
        }

        public void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Action<FactTreeMemento> callback)
        {
            FactTree pivot = Translate.MementoToFactTree(pivotTree);
            _synchronizationService.BeginGet(pivot, pivotId.key, timestamp.Key, delegate(IAsyncResult result)
            {
                FactTree factTree = _synchronizationService.EndGet(result);
                callback(Translate.FactTreeToMemento(factTree));
            }, null);
        }

        public void BeginPost(FactTreeMemento messageBody, Action callback)
        {
            _synchronizationService.BeginPost(Translate.MementoToFactTree(messageBody), delegate(IAsyncResult result)
            {
                _synchronizationService.EndPost(result);
                callback();
            }, null);
        }
    }
}
