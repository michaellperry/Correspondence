using System.Collections.Generic;
using Correspondence.Mementos;
using Correspondence.Strategy;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
{
    public class MockCommunicationStrategy : ICommunicationStrategy
    {
        private Queue<FactTreeMemento> _mockMessageBodies = new Queue<FactTreeMemento>();
        private FactTreeMemento _pivotTree = null;

        public MockCommunicationStrategy()
        {
        }

        public MockCommunicationStrategy Returns(FactTreeMemento mockMessageBody)
        {
            _mockMessageBodies.Enqueue(mockMessageBody);
            return this;
        }

        public FactTreeMemento PivotTree
        {
            get { return _pivotTree; }
        }

        public string ProtocolName
        {
            get { return "mock"; }
        }

        public string PeerName
        {
            get { return "test"; }
        }

        public void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages)
        {
        }


        public async Task<GetResultMemento> GetAsync(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            _pivotTree = pivotTree;
            return new GetResultMemento(_mockMessageBodies.Dequeue(), new TimestampID(0, 0));
        }
    }
}
