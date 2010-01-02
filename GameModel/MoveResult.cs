using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class MoveResult : CorrespondenceFact
    {
        public static Role<Move> RoleMove = new Role<Move>("move");

        private PredecessorObj<Move> _move;

        [CorrespondenceField]
        private int _result;

        public MoveResult(Move move, int result)
        {
            _move = new PredecessorObj<Move>(this, RoleMove, move);
            _result = result;
        }

        public MoveResult(FactMemento memento)
        {
            _move = new PredecessorObj<Move>(this, RoleMove, memento);
        }

        public Move Move
        {
            get { return _move.Fact; }
        }

        public int Result
        {
            get { return _result; }
        }
    }
}
