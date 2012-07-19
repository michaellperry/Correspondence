using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using Predassert;

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
    }
}
