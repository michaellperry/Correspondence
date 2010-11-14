﻿using System;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.WebService.Contract;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : ICommunicationStrategy
    {
        private IServiceClientFactory<ISynchronizationService> _synchronizationService =
            new ServiceClientFactory<ISynchronizationService>();
        private Guid _clientGuid = Guid.NewGuid();

        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get { return _synchronizationService.Address.ToString(); }
        }

        public FactTreeMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            FactTree pivot = Translate.MementoToFactTree(pivotTree);
            FactTree result = _synchronizationService.CallService(service => service.Get(pivot, pivotId.key, timestamp.Key, _clientGuid));
            return Translate.FactTreeToMemento(result);
        }

        public void Post(FactTreeMemento messageBody)
        {
            _synchronizationService.CallService(service => service.Post(Translate.MementoToFactTree(messageBody), _clientGuid));
        }
    }
}
