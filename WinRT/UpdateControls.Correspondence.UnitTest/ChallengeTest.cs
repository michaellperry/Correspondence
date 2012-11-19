using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;
using Predassert;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    [TestClass]
    public class ChallengeTest
    {
        private Community _community;
        private User _alan;
        private User _flynn;

        [TestInitialize]
        public async Task Initialize()
        {
            _community = new Community(new MemoryStorageStrategy())
                .Register<Model.CorrespondenceModel>();

            _alan = await _community.AddFactAsync(new User("alan1"));
            _flynn = await _community.AddFactAsync(new User("flynn1"));
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

            Pred.Assert(_alan,
                Has<User>.Property(u => u.ActivePlayers, Contains<Player>.That(
                    Is.SameAs(player)
                ))
            );
        }

        [TestMethod]
        public async Task OpponentSeesTheGame()
        {
            Player player = await _alan.ChallengeAsync(_flynn);

            Pred.Assert(_flynn,
                Has<User>.Property(u => u.ActivePlayers, Contains<Player>.That(
                    Has<Player>.Property(p => p.Game, Is.SameAs(player.Game)) &
                    Has<Player>.Property(p => p.Index, Is.EqualTo(1))
                ))
            );
        }
    }
}
