using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Predassert;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.UnitTest.Model;

namespace UpdateControls.Correspondence.UnitTest
{
	[TestClass]
	public class TwoPlayerGameTest
	{
        private User _userAlan;
        private User _userFlynn;
        private Community _communityAlan;
        private Community _communityFlynn;
        private Player _playerAlan;
        private Player _playerFlynn;

        [TestInitialize]
        public async Task Initialize()
        {
            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _userAlan)
                .Subscribe(() => _userAlan.ActivePlayers.Select(player => player.Game));
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .Register<Model.CorrespondenceModel>()
                .Subscribe(() => _userFlynn)
                .Subscribe(() => _userFlynn.ActivePlayers.Select(player => player.Game));

            _userAlan = await _communityAlan.AddFactAsync(new User("alan1"));
            _userFlynn = await _communityFlynn.AddFactAsync(new User("flynn1"));
            _playerAlan = await _userAlan.ChallengeAsync(await _communityAlan.AddFactAsync(new User("flynn1")));
            await Synchronize();
            _playerFlynn = _userFlynn.ActivePlayers.Single();
        }

        [TestMethod]
        public void GameHasNoMoves()
        {
            Pred.Assert(_playerAlan.Game,
                Has<Game>.Property(g => g.Moves, Is.Empty<Move>())
            );
        }

        [TestMethod]
        public void FlynnMakesAMove()
        {
            _playerAlan.MakeMove(0, 0);

            Pred.Assert(_playerAlan.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(0)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(0))
                ))
            );
        }

        [TestMethod]
        public async Task FlynnSeesAlansMove()
        {
            _playerAlan.MakeMove(0, 0);
            await Synchronize();

            Pred.Assert(_playerFlynn.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(0)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(0)) &
                    Has<Move>.Property(m => m.Player,
                        Has<Player>.Property(player => player.User,
                            Has<User>.Property(user => user.UserName, Is.EqualTo("alan1"))
                        ))
                ))
            );
        }

        [TestMethod]
        public void FlynnMakesAMoveInResponse()
        {
            _playerAlan.MakeMove(0, 0);
            _playerFlynn.MakeMove(1, 42);

            Pred.Assert(_playerFlynn.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(1)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(42))
                ))
            );
        }

        [TestMethod]
        public async Task AlanSeesFlynnsResponse()
        {
            _playerAlan.MakeMove(0, 0);
            await Synchronize();
            _playerFlynn.MakeMove(1, 42);
            await Synchronize();

            Pred.Assert(_playerAlan.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(1)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(42)) &
                    Has<Move>.Property(m => m.Player,
                        Has<Player>.Property(player => player.User,
                            Has<User>.Property(user => user.UserName, Is.EqualTo("flynn1"))
                        ))
                ))
            );
        }

        private async Task Synchronize()
        {
            while (
                await _communityAlan.SynchronizeAsync() ||
                await _communityFlynn.SynchronizeAsync()) ;
        }
	}
}
