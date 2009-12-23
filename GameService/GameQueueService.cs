using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameModel;

namespace GameService
{
    public class GameQueueService
    {
        private GameQueue _queue;

        public GameQueueService(GameQueue queue)
        {
            _queue = queue;
        }

        public IEnumerable<GameRequest> Queue
        {
            get { return _queue.OutstandingGameRequests; }
        }
    }
}
