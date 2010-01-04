using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.NetworkSimulator;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RemoteGameServiceTest
    {
        public TestContext TestContext { get; set; }

        private ICommunicationStrategy _network;
        private Server _server;
        private Client _alice;
        private Client _bob;

        [TestInitialize]
        public void Initialize()
        {
            _network = new SimulatedServer(new MemoryStorageStrategy())
                .AddPivot(
                    new RoleMemento(
                        new CorrespondenceFactType("GameModel.GameRequest", 1),
                        "gameQueue",
                        new CorrespondenceFactType("GameModel.GameQueue", 1)
                    )
                )
                .AddPivot(
                    new RoleMemento(
                        new CorrespondenceFactType("GameModel.Game", 1),
                        "gameRequest",
                        new CorrespondenceFactType("GameModel.GameRequest", 1)
                    )
                )
                .AddPivot(
                    new RoleMemento(
                        new CorrespondenceFactType("GameModel.Move", 1),
                        "game",
                        new CorrespondenceFactType("GameModel.Game", 1)
                    )
                );
            _server = new Server(_network);
            _alice = new Client(_network);
            _bob = new Client(_network);
        }

        [TestMethod]
        public void RemoteServiceQueueIsEmpty()
        {
            _server.AssertQueueCount(0);
        }

        [TestMethod]
        public void RemoteServiceReceivesGameRequest()
        {
            _alice.CreateGameRequest();
            Synchronize();
            _server.AssertQueueCount(1);
        }

        [TestMethod]
        public void RemoteServiceReceivesTwoGameRequests()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            Synchronize();
            _server.AssertQueueCount(2);
        }

        [TestMethod]
        public void RemoteServiceCreatesAGame()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            Synchronize();
            _server.RunService();
            _server.AssertQueueCount(0);
        }

        [TestMethod]
        public void ClientsReceiveGame()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            Synchronize();
            _server.RunService();
            Synchronize();
            _alice.AssertHasGame();
            _bob.AssertHasGame();
        }

        private void Synchronize()
        {
            bool any;
            do
            {
                any = false;
                if (_alice.Synchronize())
                    any = true;
                if (_bob.Synchronize())
                    any = true;
                if (_server.Synchronize())
                    any = true;
            } while (any);
        }
    }
}
