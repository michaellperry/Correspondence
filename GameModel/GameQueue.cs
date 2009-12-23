using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class GameQueue : CorrespondenceFact
    {
        private static Query QueryOutstandingGameRequests = new Query()
            .JoinSuccessors(GameRequest.RoleGameQueue, GameRequest.IsOutstanding);

        private Result<GameRequest> _outstandingGameRequests;

        [CorrespondenceField]
        private string _identifier;

        private GameQueue()
        {
            _outstandingGameRequests = new Result<GameRequest>(this, QueryOutstandingGameRequests);
        }

        public GameQueue(string identifier)
            : this()
        {
            _identifier = identifier;
        }

        public GameQueue(Memento memento)
            : this()
        {
        }

        public GameRequest CreateGameRequest(Person person)
        {
            return Community.AddFact(new GameRequest(this, person));
        }

        public IEnumerable<GameRequest> OutstandingGameRequests
        {
            get { return _outstandingGameRequests; }
        }
    }
}
