using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class GameRequest : CorrespondenceFact
    {
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue", RoleRelationship.Pivot);
        public static Role<Person> RolePerson = new Role<Person>("person");

        private static Query QueryGame = new Query()
            .JoinSuccessors(Game.RoleGameRequest);

        public static Condition IsOutstanding = Condition.WhereIsEmpty(QueryGame);

        private PredecessorObj<GameQueue> _gameQueue;
        private PredecessorObj<Person> _person;

        private Result<Game> _game;

        [CorrespondenceField]
        private Guid _unique;

        private GameRequest()
        {
            _game = new Result<Game>(this, QueryGame);
        }

        public GameRequest(GameQueue gameQueue, Person person, Guid unique)
            : this()
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
            _person = new PredecessorObj<Person>(this, RolePerson, person);
            _unique = unique;
        }

        public GameRequest(GameQueue gameQueue, Person person)
            : this(gameQueue, person, Guid.NewGuid())
        {
        }

        public GameRequest(FactMemento memento)
            : this()
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, memento);
            _person = new PredecessorObj<Person>(this, RolePerson, memento);
        }

        public GameQueue GameQueue
        {
            get { return _gameQueue.Fact; }
        }

        public Person Person
        {
            get { return _person.Fact; }
        }

        public Game CreateGame(GameRequest second)
        {
            return Community.AddFact(new Game(this, second));
        }

        public Game Game
        {
            get { return _game.FirstOrDefault(); }
        }

        public Guid Unique
        {
            get { return _unique; }
        }
    }
}
