using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.NetworkSimulator;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class RemoteGameServiceTest
    {
        public TestContext TestContext { get; set; }

        private SimulatedNetwork _context;
        private Client _alice;
        private Client _bob;
        private Server _server;

        [TestInitialize]
        public void Initialize()
        {
            _context = new SimulatedNetwork();
            _alice = new Client(_context);
            _bob = new Client(_context);
            _server = new Server(_context);
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
            _context.Synchronize();
            _server.AssertQueueCount(1);
        }

        [TestMethod]
        public void RemoteServiceReceivesTwoGameRequests()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            _context.Synchronize();
            _server.AssertQueueCount(2);
        }

        [TestMethod]
        public void RemoteServiceCreatesAGame()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            _context.Synchronize();
            _server.RunService();
            _server.AssertQueueCount(0);
        }

        [TestMethod]
        public void ClientsReceiveGame()
        {
            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            _context.Synchronize();
            _server.RunService();
            _context.Synchronize();
            _alice.AssertHasGame();
            _bob.AssertHasGame();
        }
    }
}
