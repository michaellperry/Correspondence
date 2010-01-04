using System.Linq;
using GameModel;
using GameService;
using Predassert;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.UnitTest
{
    public class Server
    {
        private Community _community;
        private GameQueue _gameQueue;
        private GameQueueService _service;

        public Server(ICommunicationStrategy communicationStrategy)
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddCommunicationStrategy(communicationStrategy)
                .AddInterest(() => _gameQueue);
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _service = new GameQueueService(_gameQueue);
        }

        public void AssertQueueCount(int count)
        {
            Pred.Assert(_service.Queue.Count(), Is.EqualTo(count));
        }

        public bool Synchronize()
        {
            return _community.Synchronize();
        }

        public void RunService()
        {
            _service.Process(_service.Queue.ToList());
        }
    }
}
