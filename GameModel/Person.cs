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

        [CorrespondenceField]
        private Guid _unique;

        private Result<GameRequest> _outstandingGameRequests;

        public Person(Guid unique)
        {
            _unique = unique;
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
        }

        public Person()
            : this(Guid.NewGuid())
        {
        }

        public Person(FactMemento memento)
        {
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
        }

        public Guid Unique
        {
            get { return _unique; }
        }

        public IEnumerable<GameRequest> OutstandingGameRequests
        {
            get { return _outstandingGameRequests; }
        }
    }
}
