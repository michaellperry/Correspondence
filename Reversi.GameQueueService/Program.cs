using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameModel;
using UpdateControls.Correspondence.Service;
using UpdateControls;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.WebServiceClient;
using System.Threading;

namespace Reversi.GameQueueService
{
    class Program
    {
        private GameQueue _gameQueue;
        private IService<GameRequest> _service;
        private Dependent _depService;

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            Community community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(new WebServiceCommunicationStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddInterest(() => _gameQueue)
                .AddInterest(() => _gameQueue.OutstandingGameRequests);

            _gameQueue = community.AddFact(new GameQueue("http://correspondence.cloudapp.net/reversi/1"));

            _service = new GameService.GameQueueService(_gameQueue);
            _depService = new Dependent(RunService);
            while (true)
            {
                community.Synchronize();
                _depService.OnGet();
                Thread.Sleep(1000);
            }
        }

        private void RunService()
        {
            List<GameRequest> queue = _service.Queue.ToList();
            queue.Reverse();
            _service.Process(queue);
        }
    }
}
