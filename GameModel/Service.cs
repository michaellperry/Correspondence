using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;

namespace GameModel
{
    [CorrespondenceType]
    public class Service : CorrespondenceFact
    {
        public static Role<GameQueue> RoleGameQueue = new Role<GameQueue>("gameQueue", RoleRelationship.Pivot);

        private PredecessorObj<GameQueue> _gameQueue;

        public Service(GameQueue gameQueue)
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, gameQueue);
        }

        public Service(FactMemento memento)
        {
            _gameQueue = new PredecessorObj<GameQueue>(this, RoleGameQueue, memento);
        }

        public GameQueue GameQueue
        {
            get { return _gameQueue.Fact; }
        }
    }
}
