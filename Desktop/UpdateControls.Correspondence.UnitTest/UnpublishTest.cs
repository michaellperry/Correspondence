using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class UnpublishTest
    {
        private Community _community;
        private Community _remoteCommunity;
        private User _alan;
        private User _flynn;
        private User _remoteAlan;

        [TestInitialize]
        public void Initialize()
        {
            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _alan)
                .Subscribe(() => _alan.ActivePlayers.Select(player => player.Game))
                ;
            _remoteCommunity = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _remoteAlan)
                .Subscribe(() => _remoteAlan.ActivePlayers.Select(player => player.Game))
                ;

            _alan = _community.AddFact(new User("alan1"));
            _flynn = _community.AddFact(new User("flynn1"));
            _remoteAlan = _remoteCommunity.AddFact(new User("alan1"));
            Player playerAlan = _alan.Challenge(_flynn);
            Player playerFlynn = _flynn.ActivePlayers.Single();

            playerAlan.MakeMove(0, 0);
            playerFlynn.MakeMove(1, 42);
        }

        [TestMethod]
        public void PlayerIsPublished()
        {
            Synchronize();
            SynchronizeRemote();

            Assert.AreEqual(1, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(0, _remoteAlan.FinishedPlayers.Count());
        }

        [TestMethod]
        public void WhenOutcomePostedAfterFetch_RemoteSeesOutcome()
        {
            Synchronize();
            SynchronizeRemote();
            PostOutcome();
            Synchronize();
            SynchronizeRemote();

            Assert.AreEqual(0, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(1, _remoteAlan.FinishedPlayers.Count());
        }

        [TestMethod]
        public void WhenOutcomePostedBeforeFetch_PlayerIsUnpublished()
        {
            Synchronize();
            PostOutcome();
            Synchronize();
            SynchronizeRemote();

            Assert.AreEqual(0, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(0, _remoteAlan.FinishedPlayers.Count());
        }

        private void PostOutcome()
        {
            Player playerFlynn = _flynn.ActivePlayers.Single();
            Game game = playerFlynn.Game;
            game.DeclareWinner(playerFlynn);
        }

        private void Synchronize()
        {
            while (_community.Synchronize()) ;
        }

        private void SynchronizeRemote()
        {
            while (_remoteCommunity.Synchronize()) ;
        }
    }
}
