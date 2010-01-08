using System;
using System.Linq;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.WebService.Contract;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : ICommunicationStrategy
    {
        private IServiceClientFactory<ISynchronizationService> _synchronizationService =
            new ServiceClientFactory<ISynchronizationService>();

        public string ProtocolName
        {
            get { throw new NotImplementedException(); }
        }

        public string PeerName
        {
            get { throw new NotImplementedException(); }
        }

        public FactTreeMemento Get(FactTreeMemento rootTree, FactID rootId, TimestampID timestamp)
        {
            FactTree root = Translate.MementoToFactTree(rootTree);
            FactTree result = _synchronizationService.CallService(service => service.Get(root, rootId.key, timestamp.key));
            return Translate.FactTreeToMemento(result);
        }

        public void Post(FactTreeMemento messageBody)
        {
            _synchronizationService.CallService(service => service.Post(Translate.MementoToFactTree(messageBody)));
        }
    }
}
