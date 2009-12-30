using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        public static Role<GameRequest> RoleGameRequest = new Role<GameRequest>("gameRequest", RoleRelationship.Pivot);

        private PredecessorList<GameRequest> _gameRequest;

        private Game()
        {
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
    }
}
