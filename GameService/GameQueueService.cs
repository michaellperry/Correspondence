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

        public void Process(List<GameRequest> queue)
        {
            int pairs = queue.Count / 2;
            for (int i = 0; i < pairs; i++)
            {
                GameRequest first = queue[i * 2];
                GameRequest second = queue[i * 2 + 1];

                first.CreateGame(second);
            }
        }
    }
}
