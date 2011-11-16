using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    public class MockCommunicationStrategy : ICommunicationStrategy
    {
        private FactTreeMemento _mockMessageBody;

        public MockCommunicationStrategy(FactTreeMemento mockMessageBody)
        {
            _mockMessageBody = mockMessageBody;
        }

        public string ProtocolName
        {
            get { return "mock"; }
        }

        public string PeerName
        {
            get { return "test"; }
        }

        public GetResultMemento Get(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp)
        {
            return new GetResultMemento(_mockMessageBody, new TimestampID(0, 0));
        }

        public void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages)
        {
        }
    }
}
