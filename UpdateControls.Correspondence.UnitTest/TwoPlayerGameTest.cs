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
		private Community _community;
        private Player _playerAlan;
        private Player _playerFlynn;

        [TestInitialize]
		public void Initialize()
		{
			_community = new Community(new MemoryStorageStrategy())
				.RegisterAssembly(typeof(Machine));

            User alan = _community.AddFact(new User("alan1"));
            User flynn = _community.AddFact(new User("flynn1"));
            _playerAlan = alan.Challenge(flynn);
            _playerFlynn = flynn.ActivePlayers.Single();
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

            Pred.Assert(_playerFlynn.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(0)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(0)) &
                    Has<Move>.Property(m => m.Player, Is.SameAs(_playerAlan))
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
            _playerFlynn.MakeMove(1, 42);

            Pred.Assert(_playerAlan.Game,
                Has<Game>.Property(g => g.Moves, Contains<Move>.That(
                    Has<Move>.Property(m => m.Index, Is.EqualTo(1)) &
                    Has<Move>.Property(m => m.Square, Is.EqualTo(42)) &
                    Has<Move>.Property(m => m.Player, Is.SameAs(_playerFlynn))
                ))
            );
        }
	}
}
