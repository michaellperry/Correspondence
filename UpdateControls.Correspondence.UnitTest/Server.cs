using System;
using System.Linq;
using GameModel;
using GameService;
using Predassert;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.NetworkSimulator;
using UpdateControls.Correspondence.Strategy;
using System.Collections.Generic;

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
                    .Post<GameQueue>(new UrlPatternImpl<GameQueue>(
                        "^gamequeue/([_a-zA-Z0-9]+)$",
                        MatchGameQueue
                    ).UrlToFact)
                    .Get<GameRequest>(new UrlPatternImpl<GameRequest>(
                        "^gamequeue/([_a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)$",
                        MatchGameRequest
                    ).UrlToFact)
                    .Post<Game>(new UrlPatternImpl<Game>(
                        "^gamequeue/([_a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)$",
                        MatchGame
                    ).UrlToFact)
                    .Get<Game>(new UrlPatternImpl<Game>(
                        "^gamequeue/([_a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)/([-a-zA-Z0-9]+)$",
                        MatchGame
                    ).UrlToFact)
                );
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _service = new GameQueueService(_gameQueue);
        }

        private static GameQueue MatchGameQueue(IMessageRepository repository, List<string> values)
        {
            return repository.FindFact(new GameQueue(values[0]));
        }

        private static GameRequest MatchGameRequest(IMessageRepository repository, List<string> values)
        {
            GameQueue gameQueue = MatchGameQueue(repository, values);
            if (gameQueue == null)
                return null;
            return MatchGameRequest(repository, gameQueue, values[1], values[2]);
        }

        private static GameRequest MatchGameRequest(IMessageRepository repository, GameQueue gameQueue, string personId, string requestId)
        {
            Person person = repository.FindFact(new Person(new Guid(personId)));
            if (person == null)
                return null;
            GameRequest firstRequest = repository.FindFact(new GameRequest(gameQueue, person, new Guid(requestId)));
            return firstRequest;
        }

        private static Game MatchGame(IMessageRepository repository, List<string> values)
        {
            GameQueue gameQueue = MatchGameQueue(repository, values);
            if (gameQueue == null)
                return null;
            GameRequest firstRequest = MatchGameRequest(repository, gameQueue, values[1], values[2]);
            if (firstRequest == null)
                return null;
            GameRequest secondRequest = MatchGameRequest(repository, gameQueue, values[3], values[4]);
            if (secondRequest == null)
                return null;
            return repository.FindFact(new Game(firstRequest, secondRequest));
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
