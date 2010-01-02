using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameModel
{
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        public static Role<GameRequest> RoleGameRequest = new Role<GameRequest>("gameRequest", RoleRelationship.Pivot);
        private static Query QueryMoves = new Query()
            .JoinSuccessors(Move.RoleGame);
        private static Query QueryOutcome = new Query()
            .JoinSuccessors(Outcome.RoleGame);
        public static Condition IsUnfinished = Condition.WhereIsEmpty(QueryOutcome);

        private PredecessorList<GameRequest> _gameRequest;

        private Result<Move> _moves;

        private Game()
        {
            _moves = new Result<Move>(this, QueryMoves);
        }

        public Game(GameRequest first, GameRequest second)
            : this()
        {
            _gameRequest = new PredecessorList<GameRequest>(this, RoleGameRequest, new List<GameRequest> { first, second });
        }

        public Game(FactMemento memento)
            : this()
        {
            _gameRequest = new PredecessorList<GameRequest>(this, RoleGameRequest, memento);
        }

        public GameRequest First
        {
            get { return _gameRequest.ElementAt(0); }
        }

        public GameRequest Second
        {
            get { return _gameRequest.ElementAt(1); }
        }

        public Move CreateMove(Person person, int index)
        {
            return Community.AddFact(new Move(person, this, index));
        }

        public IEnumerable<Move> Moves
        {
            get { return _moves; }
        }
    }
}
