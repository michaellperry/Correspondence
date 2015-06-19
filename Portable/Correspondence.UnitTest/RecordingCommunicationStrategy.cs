using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using Correspondence.Strategy;
using Correspondence.Mementos;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
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

        public void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages)
        {
            _posted.Add(messageBody);
            _innerCommunicationStrategy.Post(messageBody, unpublishedMessages);
        }

        public IEnumerable<FactTreeMemento> Posted
        {
            get { return _posted; }
        }

        public Task<GetResultMemento> GetAsync(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            return _innerCommunicationStrategy.GetAsync(pivotTree, pivotId, timestamp);
        }
    }
}
