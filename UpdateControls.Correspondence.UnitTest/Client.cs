using System;
using Reversi.Model;
using Predassert;

namespace UpdateControls.Correspondence.UnitTest
{
    public class Client
    {
        private Community _community;
        private GameQueue _gameQueue;
        private Person _person;
        private GameRequest _gameRequest;
		private Player _player;

        public Client(Community community)
        {
            _community = community;
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _person = _community.AddFact(new Person());
        }

        public Person Person
        {
            get { return _person; }
        }

        public void CreateGameRequest()
        {
            _gameRequest = _gameQueue.CreateGameRequest(_person);
        }

		public void MakeMove(int index)
		{
			CreatePlayer().MakeMove(index, 0);
		}

		private Player CreatePlayer()
		{
			if (_player == null)
				_player = _gameRequest.Game.CreatePlayer(_person);
			return _player;
		}

        public bool Synchronize()
        {
            return _community.Synchronize();
        }

        public void AssertHasGame()
        {
            Pred.Assert(_gameRequest.Game, Is.NotNull<Game>());
        }

        public void AssertHasMove(int index, Guid personId)
        {
            Pred.Assert(_gameRequest.Game.Players, Contains<Player>.That(
				Has<Player>.Property(player => player.Person.Unique, Is.EqualTo(personId)) &
				Has<Player>.Property(player => player.Moves, Contains<Move>.That(
					Has<Move>.Property(move => move.Index, Is.EqualTo(index))
				))
			));
        }
    }
}
