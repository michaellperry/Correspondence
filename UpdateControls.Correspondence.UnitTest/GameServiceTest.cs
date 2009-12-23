using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameModel;
using UpdateControls.Correspondence.Memory;
using GameService;
using Predassert;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class GameServiceTest
    {
        public TestContext TestContext { get; set; }

        private Community _community;
        private GameQueue _gameQueue;
        private Person _alice;
        private Person _bob;
        private GameQueueService _service;

        [TestInitialize]
        public void Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue));
            _alice = _community.AddFact(new Person());
            _bob = _community.AddFact(new Person());
            _gameQueue = _community.AddFact(new GameQueue("http://mydomain.com/mygamequeue"));
            _service = new GameQueueService(_gameQueue);
        }

        [TestMethod]
        public void ServiceQueueIsInitiallyEmpty()
        {
            IEnumerable<GameRequest> queue = _service.Queue;

            Pred.Assert(queue, Is.Empty<GameRequest>());
        }

        [TestMethod]
        public void ServiceQueueContainsGameRequest()
        {
            GameRequest bobsGameRequest = _gameQueue.CreateGameRequest(_bob);
            IEnumerable<GameRequest> queue = _service.Queue;

            Pred.Assert(queue, Contains<GameRequest>.That(Is.SameAs(bobsGameRequest)));
        }
    }
}
