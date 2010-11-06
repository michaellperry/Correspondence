using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using Reversi.Model;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.UnitTest
{
    public class RecordingCommunicationStrategy : ICommunicationStrategy
    {
        private ICommunicationStrategy _innerCommunicationStrategy;
        private List<FactTreeMemento> _posted = new List<FactTreeMemento>();

        public RecordingCommunicationStrategy(ICommunicationStrategy innerCommunicationStrategy)
        {
            _innerCommunicationStrategy = innerCommunicationStrategy;
        }

        public string ProtocolName
        {
            get { return _innerCommunicationStrategy.ProtocolName; }
        }

        public string PeerName
        {
            get { return _innerCommunicationStrategy.PeerName; }
        }

        public FactTreeMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            return _innerCommunicationStrategy.Get(pivotTree, pivotId, timestamp);
        }

        public void Post(FactTreeMemento messageBody)
        {
            _posted.Add(messageBody);
            _innerCommunicationStrategy.Post(messageBody);
        }

        public IEnumerable<FactTreeMemento> Posted
        {
            get { return _posted; }
        }
    }
}