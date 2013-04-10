using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using Predassert;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class AsyncNonNullTest
    {
        private Community _community;
        private AsyncMemoryStorageStrategy _memory;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public async Task Initialize()
        {
            _memory = new AsyncMemoryStorageStrategy();
            var initializingCommunity = new Community(_memory)
                .Register<Model.CorrespondenceModel>();
            var alan = await initializingCommunity.AddFactAsync(new User("alan1"));
            var flynn = await initializingCommunity.AddFactAsync(new User("flynn1"));
            var game = await initializingCommunity.AddFactAsync(new Game(null));
            await initializingCommunity.AddFactAsync(new Player(alan, game, 0));
            await initializingCommunity.AddFactAsync(new Player(flynn, game, 1));

            _community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = await _community.AddFactAsync(new User("alan1"));
            _flynn = await _community.AddFactAsync(new User("flynn1"));
        }

        [TestMethod]
        public async Task PredecessorIsInitiallyNotLoadedObject()
        {
            Player player = await GetAlansPlayer();

            var game = player.Game;

            Assert.IsNotNull(game);
            Assert.IsFalse(game.IsLoaded);
            Assert.IsFalse(game.IsNull);
        }

        [TestMethod]
        public async Task PropertiesOfNotLoadedObjectAreDefault()
        {
            Player player = await GetAlansPlayer();

            var game = player.Game;

            var unique = game.Unique;

            Assert.AreEqual(default(Guid), unique);
        }

        [TestMethod]
        public async Task QueriesOfNotLoadedObjectsAreEmpty()
        {
            Player player = await GetAlansPlayer();
            var game = player.Game;
            var moves = game.Moves;
            Assert.IsFalse(moves.Any());
        }

        [TestMethod]
        public async Task PredecessorsOfNotLoadedObjectsAreNotLoadedObjects()
        {
            Player player = await GetAlansPlayer();
            var tournament = player.Game.Tournament;
            Assert.IsFalse(tournament.IsLoaded);
        }

        [TestMethod]
        public async Task PredecessorIsEventuallyLoaded()
        {
            Player player = await GetAlansPlayer();

            var game = player.Game;

            _memory.Quiesce();

            game = player.Game;

            Assert.IsNotNull(game);
            Assert.IsTrue(game.IsLoaded);
            Assert.IsFalse(game.IsNull);
        }

        [TestMethod]
        public async Task PredecessorOfPredecessorIsEventuallyLoaded()
        {
            Player player = await GetAlansPlayer();
            var tournament = player.Game.Tournament;
            _memory.Quiesce();
            tournament = player.Game.Tournament;

            Assert.IsNotNull(tournament);
            Assert.IsTrue(tournament.IsLoaded);
            Assert.IsTrue(tournament.IsNull);
        }

        [TestMethod]
        public async Task CanEnsurePredecessor()
        {
            Player player = await GetAlansPlayer();

            QuiescePeriodically();
            Game game = null;
            try
            {
                game = await player.Game.EnsureAsync();
            }
            catch
            {
                Done();
            }

            Assert.IsNotNull(game);
            Assert.IsTrue(game.IsLoaded);
            Assert.IsFalse(game.IsNull);
        }

        private async Task<Player> GetAlansPlayer()
        {
            QuiescePeriodically();

            try
            {
                var players = await _alan.ActivePlayers.EnsureAsync();
                return players.Single();
            }
            finally
            {
                Done();
            }
        }

        private bool _done = false;
        private Task _background;

        private void QuiescePeriodically()
        {
            _background = Task.Run(async delegate
            {
                while (!_done)
                {
                    await Task.Delay(100);
                    _memory.Quiesce();
                }
            });
        }

        private void Done()
        {
            _done = true;
            _background.Wait();
        }
    }
}
