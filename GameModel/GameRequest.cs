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
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue");
        public static Role<Person> RolePerson = new Role<Person>("person");

        private static Query QueryGame = new Query()
            .JoinSuccessors(Game.RoleGameQueue);

        public static Condition IsOutstanding = Condition.WhereIsEmpty(QueryGame);

        private PredecessorObj<GameQueue> _gameQueue;
        private PredecessorObj<Person> _person;

        [CorrespondenceField]
        private Guid _unique;

        private GameRequest()
        {
        }

        public GameRequest(GameQueue gameQueue, Person person)
            : this()
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
            _person = new PredecessorObj<Person>(this, RolePerson, person);
            _unique = Guid.NewGuid();
        }

        public GameRequest(Memento memento)
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
    }
}
