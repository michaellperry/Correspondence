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
    public class AsyncChallengeTest
    {
        private Community _community;
        private AsyncMemoryStorageStrategy _memory;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public void Initialize()
        {
            _memory = new AsyncMemoryStorageStrategy();
            _community = new Community(_memory)
                .Register<Model.CorrespondenceModel>();

            _alan = _community.AddFact(new User("alan1"));
            _flynn = _community.AddFact(new User("flynn1"));
        }

        [TestMethod]
        public void UserHasNoGames()
        {
            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Is.Empty<Player>())
            );
        }

        [TestMethod]
        public async Task UserStartsAGame()
        {
            Player player = await _alan.ChallengeAsync(_flynn);

            // Still empty.
            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Is.Empty<Player>())
            );

            _memory.Quiesce();

            // Not empty anymore.
            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Contains<Player>.That(
                    Is.SameAs(player)
                ))
            );
        }

        [TestMethod]
        public async Task UserStartsAGame_Ensured()
        {
            QuiescePeriodically();

            try
            {
                Player player = await _alan.ChallengeAsync(_flynn);

                // Ensure that we have loaded the players.
                var players = _alan.ActivePlayers.Ensure();
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
            Player player = await _alan.ChallengeAsync(_flynn);

            // Not yet.
            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Is.Empty<Player>())
            );

            _memory.Quiesce();

            // Now.
            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Contains<Player>.That(
                    Has<Player>.Property(p => p.Game, Is.SameAs(player.Game)) &
                    Has<Player>.Property(p => p.Index, Is.EqualTo(1))
                ))
            );
        }

        [TestMethod]
        public void PropertyIsInconsistent()
        {
            QuiescePeriodically();

            try
            {
                _flynn.FavoriteColor = "Blue";

                // It's still blank.
                Assert.AreEqual(null, _flynn.FavoriteColor.Value);

                _memory.Quiesce();

                // Now it's set.
                Assert.AreEqual("Blue", _flynn.FavoriteColor.Value);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public void EnsuredPropertyIsConsistent()
        {
            QuiescePeriodically();

            try
            {
                _flynn.FavoriteColor = "Blue";

                // Ensure that the value is loaded.
                Assert.AreEqual("Blue", _flynn.FavoriteColor.Ensure().Value);
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
