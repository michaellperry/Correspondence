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
    public class ChallengeTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public void Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(Game));

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

            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Contains<Player>.That(
                    Has<Player>.Property(p => p.Game, Is.SameAs(player.Game)) &
                    Has<Player>.Property(p => p.Index, Is.EqualTo(1))
                ))
            );
        }
    }
}
