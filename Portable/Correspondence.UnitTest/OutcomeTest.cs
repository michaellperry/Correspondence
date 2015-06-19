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
    public class OutcomeTest
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
            Player playerAlan = await _alan.ChallengeAsync(_flynn);
            Player playerFlynn = _flynn.ActivePlayers.Single();

            playerAlan.MakeMove(0, 0);
            playerFlynn.MakeMove(1, 42);
        }

        [TestMethod]
        public async Task GameIsActiveForAlan()
        {
            await Initialize();
            Assert.AreEqual(1, _alan.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task GameIsActiveForFlynn()
        {
            await Initialize();
            Assert.AreEqual(1, _flynn.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task AfterAlanWinsGameIsNoLongerActiveForAlan()
        {
            await Initialize();
            Player alanPlayer = _alan.ActivePlayers.Single();
            alanPlayer.Game.DeclareWinner(alanPlayer);

            Assert.AreEqual(0, _alan.ActivePlayers.Count());
        }

        [TestMethod]
        public async Task AfterAlanWinsGameIsNoLongerActiveForFlynn()
        {
            await Initialize();
            Player alanPlayer = _alan.ActivePlayers.Single();
            alanPlayer.Game.DeclareWinner(alanPlayer);

            Assert.AreEqual(0, _flynn.ActivePlayers.Count());
        }
    }
}
