using System;
using System.Linq;
using GameModel;
using UpdateControls.Correspondence.Memory;
using GameService;
using Predassert;
using UpdateControls.Correspondence.NetworkSimulator;

namespace UpdateControls.Correspondence.UnitTest
{
    public class Server
    {
        private Community _community;
        private GameQueue _gameQueue;
        private GameQueueService _service;

        public Server(SimulatedNetwork network)
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddCommunicationStrategy(new SimulatedServer(network));
            _gameQueue = _community.AddFact(new GameQueue("http://mydomain.com/mygamequeue"));
            _service = new GameQueueService(_gameQueue);
        }

        public void AssertQueueCount(int count)
        {
            Pred.Assert(_service.Queue.Count(), Is.EqualTo(count));
        }
    }
}
