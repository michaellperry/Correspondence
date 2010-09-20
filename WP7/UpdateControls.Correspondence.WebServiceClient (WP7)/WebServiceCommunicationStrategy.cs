using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class WebServiceCommunicationStrategy : ICommunicationStrategy
    {
        private SynchronizationServiceClient _synchronizationService = new SynchronizationServiceClient();

        public string ProtocolName
        {
            get { return "http://correspondence.updatecontrols.net"; }
        }

        public string PeerName
        {
            get { return _synchronizationService.Endpoint.Address.ToString(); }
        }

        public FactTreeMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            throw new NotImplementedException();
            //FactTree pivot = Translate.MementoToFactTree(pivotTree);
            //FactTree result = _synchronizationService.CallService(service => service.Get(pivot, pivotId.key, timestamp.Key));
            //return Translate.FactTreeToMemento(result);
        }

        public void Post(FactTreeMemento messageBody)
        {
            throw new NotImplementedException();
            //_synchronizationService.CallService(service => service.Post(Translate.MementoToFactTree(messageBody)));
        }
    }
}
