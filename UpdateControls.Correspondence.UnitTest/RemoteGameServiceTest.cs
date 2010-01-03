using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.NetworkSimulator;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RemoteGameServiceTest
    {
        public TestContext TestContext { get; set; }

        private Server _server;
        private Client _alice;
        private Client _bob;

        [TestInitialize]
        public void Initialize()
        {
            _server = new Server();
            _alice = new Client(_server.SimulatedServer);
            _bob = new Client(_server.SimulatedServer);
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
            } while (any);
        }
    }
}
