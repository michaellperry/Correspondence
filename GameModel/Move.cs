using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class Move : CorrespondenceFact
    {
        public static Role<GameRequest> RoleGameRequest = new Role<GameRequest>("gameRequest");
        public static Role<Game> RoleGame = new Role<Game>("game", RoleRelationship.Pivot);

        private PredecessorObj<GameRequest> _gameRequest;
        private PredecessorObj<Game> _game;

        [CorrespondenceField]
        private int _index;

        public Move(GameRequest gameRequest, Game game, int index)
        {
            _gameRequest = new PredecessorObj<GameRequest>(this, RoleGameRequest, gameRequest);
            _game = new PredecessorObj<Game>(this, RoleGame, game);
            _index = index;
        }

        public Move(FactMemento memento)
        {
            _gameRequest = new PredecessorObj<GameRequest>(this, RoleGameRequest, memento);
            _game = new PredecessorObj<Game>(this, RoleGame, memento);
        }

        public GameRequest GameRequest
        {
            get { return _gameRequest.Fact; }
        }

        public Game Game
        {
            get { return _game.Fact; }
        }

        public int Index
        {
            get { return _index; }
        }
    }
}
