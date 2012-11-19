using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using Predassert;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class OutcomeTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public void Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .Register<Model.CorrespondenceModel>();

            _alan = _community.AddFact(new User("alan1"));
            _flynn = _community.AddFact(new User("flynn1"));
            Player playerAlan = _alan.Challenge(_flynn);
            Player playerFlynn = _flynn.ActivePlayers.Single();

            playerAlan.MakeMove(0, 0);
            playerFlynn.MakeMove(1, 42);
        }

        [TestMethod]
        public void GameIsActiveForAlan()
        {
            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Is.NotEmpty<Player>())
            );
        }

        [TestMethod]
        public void GameIsActiveForFlynn()
        {
            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Is.NotEmpty<Player>())
            );
        }

        [TestMethod]
        public void AfterAlanWinsGameIsNoLongerActiveForAlan()
        {
            Player alanPlayer = _alan.ActivePlayers.Single();
            alanPlayer.Game.DeclareWinner(alanPlayer);

            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Is.Empty<Player>())
            );
        }

        [TestMethod]
        public void AfterAlanWinsGameIsNoLongerActiveForFlynn()
        {
            Player alanPlayer = _alan.ActivePlayers.Single();
            alanPlayer.Game.DeclareWinner(alanPlayer);

            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Is.Empty<Player>())
            );
        }
    }
}
