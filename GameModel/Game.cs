using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using System.Collections.Generic;
using System.Linq;

namespace GameModel
{
    [CorrespondenceType]
    public class Game : CorrespondenceFact
    {
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue");

        private PredecessorObj<GameQueue> _gameQueue;

        private Game()
        {
        }

        public Game(GameQueue gameQueue)
            : this()
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
        }

        public Game(Memento memento)
            : this()
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, memento);
        }

        public GameQueue GameQueue
        {
            get { return _gameQueue.Fact; }
        }
    }
}
