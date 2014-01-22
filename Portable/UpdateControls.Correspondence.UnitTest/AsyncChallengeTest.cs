using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class AsyncChallengeTest : AsyncTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        public async Task Initialize()
        {
            InitializeAsyncTest();
            _community = new Community(_memory)
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

            // Still empty.
            Assert.AreEqual(0, _alan.ActivePlayers.Count());

            await _memory.Quiesce();

            // Not empty anymore.
            Assert.AreEqual(1, _alan.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task UserStartsAGame_Ensured()
        {
            await Initialize();
            QuiescePeriodically();

            try
            {
                Player player = await _alan.ChallengeAsync(_flynn);

                // Ensure that we have loaded the players.
                var players = await _alan.ActivePlayers.EnsureAsync();
                Assert.IsTrue(players.Any(), "The collection is still empty.");
                Assert.IsTrue(players.Contains(player));
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public async Task OpponentSeesTheGame()
        {
            await Initialize();
            Player player = await _alan.ChallengeAsync(_flynn);

            // Not yet.
            Assert.AreEqual(0, _flynn.ActivePlayers.Count());

            await _memory.Quiesce();

            // Now.
            Assert.AreEqual(1, _flynn.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task PropertyIsInconsistent()
        {
            await Initialize();
            QuiescePeriodically();

            try
            {
                _flynn.FavoriteColor = "Blue";

                // It's still blank.
                Assert.AreEqual(null, _flynn.FavoriteColor.Value);

                await _memory.Quiesce();

                // Now it's set.
                Assert.AreEqual("Blue", _flynn.FavoriteColor.Value);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public async Task EnsuredPropertyIsConsistent()
        {
            await Initialize();
            QuiescePeriodically();

            try
            {
                _flynn.FavoriteColor = "Blue";

                await Task.Delay(100);

                // Ensure that the value is loaded.
                Assert.AreEqual("Blue", (await _flynn.FavoriteColor.EnsureAsync()).Value);
            }
            finally
            {
                Done();
            }
        }
    }
}
