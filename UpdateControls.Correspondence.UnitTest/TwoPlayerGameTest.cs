using GameModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.NetworkSimulator;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    /// <summary>
    /// Summary description for TwoPlayerGameTest
    /// </summary>
    [TestClass]
    public class TwoPlayerGameTest
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
                if (_server.Synchronize())
                    any = true;
            } while (any);
        }
    }
}
