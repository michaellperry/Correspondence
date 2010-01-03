using System;
using System.Linq;
using GameModel;
using GameService;
using Predassert;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.NetworkSimulator;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    public class Server
    {
        private SimulatedServer _simulatedServer;
        private Community _community;
        private GameQueue _gameQueue;
        private GameQueueService _service;

        public Server()
        {
            _simulatedServer = new SimulatedServer();
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddCommunicationStrategy((ICommunicationStrategy)_simulatedServer);
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _service = new GameQueueService(_gameQueue);
        }

        public SimulatedServer SimulatedServer
        {
            get { return _simulatedServer; }
        }

        public void AssertQueueCount(int count)
        {
            Pred.Assert(_service.Queue.Count(), Is.EqualTo(count));
        }

        public void RunService()
        {
            _service.Process(_service.Queue.ToList());
        }
    }
}
