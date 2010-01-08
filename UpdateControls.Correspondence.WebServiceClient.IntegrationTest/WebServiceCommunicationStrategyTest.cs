using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using Predassert;

namespace UpdateControls.Correspondence.WebServiceClient.IntegrationTest
{
    [TestClass]
    public class WebServiceCommunicationStrategyTest
    {
        public TestContext TestContext { get; set; }

        private WebServiceCommunicationStrategy _strategy;
        private long _nextId;

        [TestInitialize]
        public void Initialize()
        {
            _strategy = new WebServiceCommunicationStrategy();
            _nextId = 0;
        }

        [TestMethod]
        public void PostGameQueueToServer()
        {
            FactID gameQueueId;

            FactTreeMemento messageBody = new FactTreeMemento();
            messageBody.Add(CreateGameQueue(out gameQueueId));
            _strategy.Post(messageBody);
        }

        [TestMethod]
        public void GetFromServer()
        {
            FactID gameQueueId = PostGameQueue();

            FactTreeMemento message = GetSuccessorsOfGameQueue(gameQueueId);

            Pred.Assert(message, Is.NotNull<FactTreeMemento>());
        }

        private FactID PostGameQueue()
        {
            FactID gameQueueId;
            FactTreeMemento messageBody = new FactTreeMemento();
            messageBody.Add(CreateGameQueue(out gameQueueId));
            _strategy.Post(messageBody);
            return gameQueueId;
        }

        private FactTreeMemento GetSuccessorsOfGameQueue(FactID gameQueueId)
        {
            FactTreeMemento rootTree = new FactTreeMemento();
            rootTree.Add(CreateGameQueue(out gameQueueId));
            return _strategy.Get(rootTree, gameQueueId, new TimestampID() { key = 0 });
        }

        private FactID NewFactId()
        {
            return new FactID() { key = ++_nextId };
        }

        private IdentifiedFactMemento CreateGameQueue(out FactID gameQueueId)
        {
            gameQueueId = NewFactId();
            FactMemento memento = new FactMemento(new CorrespondenceFactType("GameModel.GameQueue", 1));
            memento.Data = new byte[] { 1, 2, 3, 4, 5 };
            IdentifiedFactMemento gameQueue = new IdentifiedFactMemento(gameQueueId, memento);
            return gameQueue;
        }
    }
}
