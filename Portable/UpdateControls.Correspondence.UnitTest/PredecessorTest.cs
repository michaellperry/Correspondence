using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class PredecessorTest
    {
        [TestMethod]
        public async Task CanSetNullPredecessorOpt()
        {
            Community community = GivenCommunity();
            var game = await GivenGame(community);

            var outcome = await community.AddFactAsync(new Outcome(game, null));

            Assert.IsNotNull(outcome.Winner);
            Assert.IsTrue(outcome.Winner.IsNull);
        }

        private static Community GivenCommunity()
        {
            var community = new Community(new MemoryStorageStrategy());
            community.Register<CorrespondenceModel>();
            return community;
        }

        private static async System.Threading.Tasks.Task<Game> GivenGame(Community community)
        {
            var game = await community.AddFactAsync(new Game());
            return game;
        }
    }
}
