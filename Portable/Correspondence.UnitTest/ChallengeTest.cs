using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using System.Threading.Tasks;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class ChallengeTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        public async Task Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .Register<Model.CorrespondenceModel>();

            _alan = await _community.AddFactAsync(new User("alan1"));
            _flynn = await _community.AddFactAsync(new User("flynn1"));
        }

        [TestMethod]
        public async Task UserHasNoGames()
        {
            await Initialize();

            Assert.AreEqual(0, _alan.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task UserStartsAGame()
        {
            await Initialize();

            Player player = await _alan.ChallengeAsync(_flynn);

            Assert.AreSame(player, _alan.ActivePlayers.Single());
        }

        [TestMethod]
        public async Task OpponentSeesTheGame()
        {
            await Initialize();

            Player player = await _alan.ChallengeAsync(_flynn);

            Player otherPlayer = _flynn.ActivePlayers.Single();
            Assert.AreSame(player.Game, otherPlayer.Game);
            Assert.AreEqual(1, otherPlayer.Index);
        }
    }
}
