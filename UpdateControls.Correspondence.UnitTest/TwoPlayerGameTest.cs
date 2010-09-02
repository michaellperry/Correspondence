using System.Linq;
using Reversi.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Memory;
using Predassert;

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
        public void Initialize()
        {
            MemoryCommunicationStrategy sharedCommunication = new MemoryCommunicationStrategy();
            _communityAlan = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .RegisterAssembly(typeof(Machine))
                .Subscribe(() => _userAlan)
                .Subscribe(() => _userAlan.ActivePlayers.Select(player => player.Game));
            _communityFlynn = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(sharedCommunication)
                .RegisterAssembly(typeof(Machine))
                .Subscribe(() => _userFlynn)
                .Subscribe(() => _userFlynn.ActivePlayers.Select(player => player.Game));

            _userAlan = _communityAlan.AddFact(new User("alan1"));
            _userFlynn = _communityFlynn.AddFact(new User("flynn1"));
            _playerAlan = _userAlan.Challenge(_communityAlan.AddFact(new User("flynn1")));
            Synchronize();
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
        public void FlynnSeesAlansMove()
        {
            _playerAlan.MakeMove(0, 0);
            Synchronize();

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
        public void AlanSeesFlynnsResponse()
        {
            _playerAlan.MakeMove(0, 0);
            Synchronize();
            _playerFlynn.MakeMove(1, 42);
            Synchronize();

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

        private void Synchronize()
        {
            while (_communityAlan.Synchronize() || _communityFlynn.Synchronize()) ;
        }
	}
}
