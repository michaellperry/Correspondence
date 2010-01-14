using System.Collections.Generic;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;

namespace GameModel
{
    [CorrespondenceType]
    public class Player : CorrespondenceFact
    {
        public static Role<Person> RolePerson = new Role<Person>("person");
        public static Role<Game> RoleGame = new Role<Game>("game", RoleRelationship.Pivot);
        private static Query QueryMove = new Query()
            .JoinSuccessors(Move.RolePlayer);

        private PredecessorObj<Person> _person;
        private PredecessorObj<Game> _game;
        private Result<Move> _moves;

        private Player()
        {
            _moves = new Result<Move>(this, QueryMove);
        }

        public Player(Person person, Game game)
            : this()
        {
            _person = new PredecessorObj<Person>(this, RolePerson, person);
            _game = new PredecessorObj<Game>(this, RoleGame, game);
        }

        public Player(FactMemento memento)
            : this()
        {
            _person = new PredecessorObj<Person>(this, RolePerson, memento);
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
        }

        public Person Person
        {
            get { return _person.Fact; }
        }

        public Game Game
        {
            get { return _game.Fact; }
        }

		public void MakeMove(int index)
		{
			Community.AddFact(new Move(this, index));
		}

        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
    }
}
