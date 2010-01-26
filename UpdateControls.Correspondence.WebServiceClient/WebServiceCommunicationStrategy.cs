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

        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get { return "http://correspondence.cloudapp.net/SynchronizationService.svc"; }
        }

        public FactTreeMemento Get(FactTreeMemento rootTree, FactID rootId, TimestampID timestamp)
        {
            FactTree root = Translate.MementoToFactTree(rootTree);
            FactTree result = _synchronizationService.CallService(service => service.Get(root, rootId.key, timestamp.Key));
            return Translate.FactTreeToMemento(result);
        }

        public void Post(FactTreeMemento messageBody)
        {
            _synchronizationService.CallService(service => service.Post(Translate.MementoToFactTree(messageBody)));
        }
    }
}