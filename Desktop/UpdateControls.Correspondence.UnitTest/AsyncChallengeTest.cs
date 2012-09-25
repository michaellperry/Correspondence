using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using Predassert;
using System.Threading;

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
        public void UserStartsAGame()
        {
            Player player = _alan.Challenge(_flynn);

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
        public void UserStartsAGame_Ensured()
        {
            QuiescePeriodically();

            try
            {
                Player player = _alan.Challenge(_flynn);

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
        public void OpponentSeesTheGame()
        {
            Player player = _alan.Challenge(_flynn);

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

                Assert.AreEqual("Blue", _flynn.FavoriteColor.Ensure().Value);
            }
            finally
            {
                Done();
            }
        }

        [TestMethod]
        public void SurvivesARaceCondition()
        {
            _alan.Challenge(_flynn);

            // Start loading.
            Assert.IsFalse(_alan.ActivePlayers.Any(), "The ActivePlayers list should be initially empty.");

            _memory.CalculateResults();

            // Make a change while it is loading.
            _flynn.Challenge(_alan);

            Assert.IsFalse(_alan.ActivePlayers.Any(), "The ActivePlayers list should still be empty.");

            _memory.DeliverResults();

            // Now we can see the first one.
            Assert.AreEqual(1, _alan.ActivePlayers.Count());

            _memory.Quiesce();

            // And now we can see both.
            Assert.AreEqual(2, _alan.ActivePlayers.Count());
        }

        private bool _done = false;
        private Thread _background;

        private void QuiescePeriodically()
        {
            _background = new Thread(delegate(object o)
            {
                while (!_done)
                {
                    Thread.Sleep(100);
                    _memory.Quiesce();
                }
            });
            _background.Start();
        }

        private void Done()
        {
            _done = true;
            _background.Join();
        }
    }
}
