using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading.Tasks;

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
        public async Task Initialize()
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

            _alan = await _community.AddFactAsync(new User("alan1"));
            _flynn = await _community.AddFactAsync(new User("flynn1"));
            _remoteAlan = await _remoteCommunity.AddFactAsync(new User("alan1"));
            Player playerAlan = await _alan.ChallengeAsync(_flynn);
            Player playerFlynn = _flynn.ActivePlayers.Single();

            playerAlan.MakeMove(0, 0);
            playerFlynn.MakeMove(1, 42);
        }

        [TestMethod]
        public async Task PlayerIsPublished()
        {
            await SynchronizeAsync();
            await SynchronizeRemoteAsync();

            Assert.AreEqual(1, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(0, _remoteAlan.FinishedPlayers.Count());
        }

        [TestMethod]
        public async Task WhenOutcomePostedAfterFetch_RemoteSeesOutcome()
        {
            await SynchronizeAsync();
            await SynchronizeRemoteAsync();
            PostOutcome();
            await SynchronizeAsync();
            await SynchronizeRemoteAsync();

            Assert.AreEqual(0, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(1, _remoteAlan.FinishedPlayers.Count());
        }

        [TestMethod]
        public async Task WhenOutcomePostedBeforeFetch_PlayerIsUnpublished()
        {
            await SynchronizeAsync();
            PostOutcome();
            await SynchronizeAsync();
            await SynchronizeRemoteAsync();

            Assert.AreEqual(0, _remoteAlan.ActivePlayers.Count());
            Assert.AreEqual(0, _remoteAlan.FinishedPlayers.Count());
        }

        private void PostOutcome()
        {
            Player playerFlynn = _flynn.ActivePlayers.Single();
            Game game = playerFlynn.Game;
            game.DeclareWinner(playerFlynn);
        }

        private async Task SynchronizeAsync()
        {
            while (await _community.SynchronizeAsync()) ;
        }

        private async Task SynchronizeRemoteAsync()
        {
            while (await _remoteCommunity.SynchronizeAsync()) ;
        }
    }
}
