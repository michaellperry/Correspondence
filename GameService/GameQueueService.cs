using System.Collections.Generic;
using GameModel;
using UpdateControls.Correspondence.Service;

namespace GameService
{
    public class GameQueueService : IService<GameRequest>
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
            GameRequest firstRequest = null;
            foreach (GameRequest request in queue)
            {
                if (firstRequest == null)
                    firstRequest = request;
                else if (firstRequest.Person != request.Person)
                {
                    firstRequest.CreateGame(request);
                    firstRequest = null;
                }
            }
        }
    }
}
