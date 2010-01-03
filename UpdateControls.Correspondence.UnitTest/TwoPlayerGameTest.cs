using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.NetworkSimulator;
using GameModel;

namespace UpdateControls.Correspondence.UnitTest
{
    /// <summary>
    /// Summary description for TwoPlayerGameTest
    /// </summary>
    [TestClass]
    public class TwoPlayerGameTest
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

            _alice.CreateGameRequest();
            _bob.CreateGameRequest();
            Synchronize();
            _server.RunService();
            Synchronize();
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
            Synchronize();

            _alice.AssertHasMove(0, _bob.Person.Unique);
        }

        [TestMethod]
        public void AliceMakesAMoveInResponse()
        {
            _bob.MakeMove(0);
            Synchronize();
            _alice.MakeMove(1);

            _alice.AssertHasMove(1, _alice.Person.Unique);
        }

        [TestMethod]
        public void BobSeesAlicesResponse()
        {
            _bob.MakeMove(0);
            Synchronize();
            _alice.MakeMove(1);
            Synchronize();

            _bob.AssertHasMove(1, _alice.Person.Unique);
        }

        [TestMethod]
        public void BobsMoveHasAResult()
        {
            Move bobsMove = _bob.MakeMove(0);
            bobsMove.Result = 3;

            _bob.AssertHasMove(0, _bob.Person.Unique, 3);
        }

        [TestMethod]
        public void AliceSeesBobsResult()
        {
            Move bobsMove = _bob.MakeMove(0);
            bobsMove.Result = 3;
            Synchronize();

            _alice.AssertHasMove(0, _bob.Person.Unique, 3);
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
