using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class Move : CorrespondenceFact
    {
        public static Role<Person> RolePerson = new Role<Person>("person");
        public static Role<Game> RoleGame = new Role<Game>("game", RoleRelationship.Pivot);
        private static Query QueryMoveResult = new Query()
            .JoinSuccessors(MoveResult.RoleMove);

        private PredecessorObj<Person> _person;
        private PredecessorObj<Game> _game;
        private Result<MoveResult> _moveResult;

        [CorrespondenceField]
        private int _index;

        private Move()
        {
            _moveResult = new Result<MoveResult>(this, QueryMoveResult);
        }

        public Move(Person person, Game game, int index)
            : this()
        {
            _person = new PredecessorObj<Person>(this, RolePerson, person);
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _index = index;
        }

        public Move(FactMemento memento)
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

        public int Index
        {
            get { return _index; }
        }

        public int Result
        {
            get { return _moveResult.Select(moveResult => moveResult.Result).FirstOrDefault(); }
            set { Community.AddFact(new MoveResult(this, value)); }
        }
    }
}
