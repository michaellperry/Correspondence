using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Memory;
using Correspondence.UnitTest.Model;
using System.Threading;
using System.Threading.Tasks;
using Assisticant.Fields;

namespace Correspondence.UnitTest
{
    [TestClass]
    public class AsyncChallengeTest : AsyncTest
    {
        private User _alan;
        private User _flynn;

        public async Task Initialize()
        {
            InitializeAsyncTest();
            Community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = await Community.AddFactAsync(new User("alan1"));
            _flynn = await Community.AddFactAsync(new User("flynn1"));
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

            // Still empty.
            Assert.AreEqual(0, _alan.ActivePlayers.Count());

            await QuiesceAllAsync();

            // Not empty anymore.
            Assert.AreEqual(1, _alan.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task UserStartsAGame_Ensured()
        {
            await Initialize();

            Player player = await _alan.ChallengeAsync(_flynn);

            // Ensure that we have loaded the players.
            var players = await _alan.ActivePlayers.EnsureAsync();
            Assert.IsTrue(players.Any(), "The collection is still empty.");
            Assert.IsTrue(players.Contains(player));
        }

        [TestMethod]
        public async Task OpponentSeesTheGame()
        {
            await Initialize();
            Player player = await _alan.ChallengeAsync(_flynn);

            // Not yet.
            Assert.AreEqual(0, _flynn.ActivePlayers.Count());

            await QuiesceAllAsync();

            // Now.
            Assert.AreEqual(1, _flynn.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task PropertyIsInconsistent()
        {
            await Initialize();

            var dependent = new Computed<string>(() =>_flynn.FavoriteColor);

            _flynn.FavoriteColor = "Blue";

            await QuiesceAllAsync();

            // Bringing the dependent up-to-date starts the background loader,
            // and returns the default value.
            Assert.AreEqual(null, dependent.Value);

            await QuiesceAllAsync();

            // When the background loader finishes, bringing the dependent back
            // up-to-date returns the loaded value.
            Assert.AreEqual("Blue", dependent.Value);
        }

        [TestMethod]
        public async Task EnsuredPropertyIsConsistent()
        {
            await Initialize();

            _flynn.FavoriteColor = "Blue";

            await QuiesceAllAsync();

            // Ensure that the value is loaded.
            Assert.AreEqual("Blue", (await _flynn.FavoriteColor.EnsureAsync()).Value);
        }
    }
}
