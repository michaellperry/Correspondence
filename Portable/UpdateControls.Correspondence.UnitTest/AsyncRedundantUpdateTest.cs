using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Fields;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using System.Threading.Tasks;
using UpdateControls.Correspondence.UnitTest.Utilities;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class AsyncRedundantUpdateTest
    {
        [TestMethod]
        public async Task UpdatesTheMinimumRequiredNumberOfTimes()
        {
            AsyncMemoryStorageStrategy storage = GivenStorage();
            Community community = GivenCommunity(storage);
            Game game = await GivenGameWithPlayers(community, 10);

            int passes = 0;

            Dependent<int> counter = new Dependent<int>(delegate
            {
                passes++;
                var players = game.Players.ToArray();
                var likes = players
                    .Count(player => player.User.FavoriteColor.Value == "Blue");
                return likes;
            });

            while (!counter.IsUpToDate)
            {
                counter.OnGet();
                await storage.RunOneTask();
                await Task.Yield();
            }

            Assert.AreEqual(3, passes);
        }

        private static AsyncMemoryStorageStrategy GivenStorage()
        {
            return new AsyncMemoryStorageStrategy();
        }
        private Community GivenCommunity(AsyncMemoryStorageStrategy storage)
        {
            return new Community(storage)
                .Register<CorrespondenceModel>();
        }

        private async Task<Game> GivenGameWithPlayers(Community community, int playerCount)
        {
            var game = await community.AddFactAsync(new Game());
            for (int playerIndex = 0; playerIndex < playerCount; ++playerIndex)
            {
                var user = await community.AddFactAsync(new User(String.Format("user{0}", playerIndex)));
                await game.CreatePlayerAsync(user);
            }
            return game;
        }
    }
}
