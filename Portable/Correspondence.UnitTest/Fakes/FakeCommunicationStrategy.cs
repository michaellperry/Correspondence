using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Correspondence.Strategy;
using System.Threading.Tasks;
using Correspondence.Mementos;

namespace Correspondence.UnitTest.Fakes
{
    public class FakeCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
        public string ProtocolName
        {
            get { return "Fake"; }
        }

        public string PeerName
        {
            get { return "Fake"; }
        }

        public Task<GetManyResultMemento> GetManyAsync(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid)
        {
            throw new NotImplementedException();
        }

        public Task PostAsync(FactTreeMemento messageBody, Guid clientGuid, List<UnpublishMemento> unpublishedMessages)
        {
            throw new NotImplementedException();
        }

        public Task InterruptAsync(Guid clientGuid)
        {
            throw new NotImplementedException();
        }

        public Task NotifyAsync(FactTreeMemento messageBody, FactID pivotId, Guid clientGuid, string text1, string text2)
        {
            throw new NotImplementedException();
        }

        public bool IsLongPolling
        {
            get { throw new NotImplementedException(); }
        }

        public event Action<FactTreeMemento> MessageReceived;

        public Task<IPushSubscription> SubscribeForPushAsync(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
        {
            throw new NotImplementedException();
        }

        public Exception LastException
        {
            get { throw new NotImplementedException(); }
        }
    }
}
