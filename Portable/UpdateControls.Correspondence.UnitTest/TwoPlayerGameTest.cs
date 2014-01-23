using System.Linq;
using UpdateControls.Correspondence.UnitTest.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
	[TestClass]
	public class TwoPlayerGameTest
	{
        private User _userAlan;
        private User _userFlynn;
        private Community _communityAlan;
        private Community _communityFlynn;
        private Player _playerAlan;
        private Player _playerFlynn;

        public async Task Initialize()
        {
            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _userAlan)
                .Subscribe(() => _userAlan.ActivePlayers.Select(player => player.Game));
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _userFlynn)
                .Subscribe(() => _userFlynn.ActivePlayers.Select(player => player.Game));

            _userAlan = await _communityAlan.AddFactAsync(new User("alan1"));
            _userFlynn = await _communityFlynn.AddFactAsync(new User("flynn1"));
            _playerAlan = await _userAlan.ChallengeAsync(await _communityAlan.AddFactAsync(new User("flynn1")));
            await Synchronize();
            _playerFlynn = _userFlynn.ActivePlayers.Single();
        }

        [TestMethod]
        public async Task GameHasNoMoves()
        {
            await Initialize();
            Assert.AreEqual(0, _playerAlan.Game.Moves.Count());
        }

        [TestMethod]
        public async Task FlynnMakesAMove()
        {
            await Initialize();

            _playerAlan.MakeMove(0, 0);

            var move = _playerAlan.Game.Moves.Single();
            Assert.AreEqual(0, move.Index);
            Assert.AreEqual(0, move.Square);
        }

        [TestMethod]
        public async Task FlynnSeesAlansMove()
        {
            await Initialize();
            _playerAlan.MakeMove(0, 0);
            await Synchronize();

            var move = _playerFlynn.Game.Moves.Single();
            Assert.AreEqual(0, move.Index);
            Assert.AreEqual(0, move.Square);
            Assert.AreEqual("alan1", move.Player.User.UserName);
        }

        [TestMethod]
        public async Task FlynnMakesAMoveInResponse()
        {
            await Initialize();

            _playerAlan.MakeMove(0, 0);
            _playerFlynn.MakeMove(1, 42);

            var move = _playerFlynn.Game.Moves.ElementAt(0);
            Assert.AreEqual(1, move.Index);
            Assert.AreEqual(42, move.Square);
        }

        [TestMethod]
        public async Task AlanSeesFlynnsResponse()
        {
            await Initialize();

            _playerAlan.MakeMove(0, 0);
            await Synchronize();
            _playerFlynn.MakeMove(1, 42);
            await Synchronize();

            var move = _playerAlan.Game.Moves.ElementAt(0);
            Assert.AreEqual(1, move.Index);
            Assert.AreEqual(42, move.Square);
            var player = await move.Player.EnsureAsync();
            var user = await player.User.EnsureAsync();
            Assert.AreEqual("flynn1", user.UserName);
        }

        private async Task Synchronize()
        {
            while (await _communityAlan.SynchronizeAsync() || await _communityFlynn.SynchronizeAsync()) ;
        }
	}
}
