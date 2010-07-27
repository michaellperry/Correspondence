using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Reversi.Model;
using UpdateControls;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.Service;
using UpdateControls.Correspondence.WebServiceClient;

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
                .Subscribe(() => _gameQueue)
                .Subscribe(() => _gameQueue.OutstandingGameRequests);

            _gameQueue = community.AddFact(new GameQueue("http://correspondence.cloudapp.net/reversi/1"));
            Service service = community.AddFact(new Service(_gameQueue));

            _service = new GameService.GameQueueService(_gameQueue);
            _depService = new Dependent(RunService);
            while (true)
            {
                try
                {
                    community.Synchronize();
                    _depService.OnGet();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
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
