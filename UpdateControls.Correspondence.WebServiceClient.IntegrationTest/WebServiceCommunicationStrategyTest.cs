using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using Predassert;

namespace UpdateControls.Correspondence.WebServiceClient.IntegrationTest
{
    [TestClass]
    public class WebServiceCommunicationStrategyTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetFromServer()
        {
            WebServiceCommunicationStrategy strategy = new WebServiceCommunicationStrategy();
            FactTreeMemento rootTree = new FactTreeMemento();
            FactID rootId = new FactID() { key = 0 };
            TimestampID timestamp = new TimestampID() { key = 0 };
            FactTreeMemento message = strategy.Get(rootTree, rootId, timestamp);

            Pred.Assert(message, Is.NotNull<FactTreeMemento>());
        }
    }
}
