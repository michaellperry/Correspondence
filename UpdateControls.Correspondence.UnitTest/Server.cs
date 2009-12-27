using System;
using System.Linq;
using GameModel;
using GameService;
using Predassert;
using UpdateControls.Correspondence.Memory;
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
                .AddCommunicationStrategy(new SimulatedServer(network)
                    .Post<GameQueue>(UrlPattern.Create(
                        "gamequeue/([_a-zA-Z0-9]+)",
                        (repository, values) => repository.AddFact(new GameQueue(values[0]))
                    ).UrlToFact)
                    .Get<GameRequest>(UrlPattern.Create(
                        "gamequeue/([_a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)",
                        (repository, values) => repository.AddFact(new GameRequest(
                            repository.AddFact(new GameQueue(values[0])),
                            repository.AddFact(new Person(new Guid(values[1]))),
                            new Guid(values[2])
                        ))
                    ).UrlToFact)
                );
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _service = new GameQueueService(_gameQueue);
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
