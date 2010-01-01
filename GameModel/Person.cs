using System;
using System.Collections.Generic;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;

namespace GameModel
{
    [CorrespondenceType]
    public class Person : CorrespondenceFact
    {
        private static Query QueryOutstandingGameRequests = new Query()
            .JoinSuccessors(GameRequest.RolePerson, GameRequest.IsOutstanding);
        private static Query QueryUnfinishedGames = new Query()
            .JoinSuccessors(GameRequest.RolePerson)
            .JoinSuccessors(Game.RoleGameRequest, Game.IsUnfinished);

        [CorrespondenceField]
        private Guid _unique;

        private Result<GameRequest> _outstandingGameRequests;
        private Result<Game> _unfinishedGames;

        public Person(Guid unique)
        {
            _unique = unique;
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
            _unfinishedGames = new Result<Game>(this, QueryUnfinishedGames);
        }

        public Person()
            : this(Guid.NewGuid())
        {
        }

        public Person(FactMemento memento)
        {
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
            _unfinishedGames = new Result<Game>(this, QueryUnfinishedGames);
        }

        public Guid Unique
        {
            get { return _unique; }
        }

        public IEnumerable<GameRequest> OutstandingGameRequests
        {
            get { return _outstandingGameRequests; }
        }

        public IEnumerable<Game> UnfinishedGames
        {
            get { return _unfinishedGames; }
        }
    }
}
