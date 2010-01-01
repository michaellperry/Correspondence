using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.NetworkSimulator;

namespace UpdateControls.Correspondence.UnitTest
{
    /// <summary>
    /// Summary description for TwoPlayerGameTest
    /// </summary>
    [TestClass]
    public class TwoPlayerGameTest
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

            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            _context.Synchronize();
            _server.RunService();
            _context.Synchronize();
        }

        [TestMethod]
        public void BobMakesAMove()
        {
            _bob.MakeMove(0);

            _bob.AssertHasMove(0, _bob.Person.Unique);
        }

        [TestMethod]
        public void AliceSeesBobsMove()
        {
            _bob.MakeMove(0);
            _context.Synchronize();

            _alice.AssertHasMove(0, _bob.Person.Unique);
        }

        [TestMethod]
        public void AliceMakesAMoveInResponse()
        {
            _bob.MakeMove(0);
            _context.Synchronize();
            _alice.MakeMove(1);

            _alice.AssertHasMove(1, _alice.Person.Unique);
        }

        [TestMethod]
        public void BobSeesAlicesResponse()
        {
            _bob.MakeMove(0);
            _context.Synchronize();
            _alice.MakeMove(1);
            _context.Synchronize();

            _bob.AssertHasMove(1, _alice.Person.Unique);
        }
    }
}
