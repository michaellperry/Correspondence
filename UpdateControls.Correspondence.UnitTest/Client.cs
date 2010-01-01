using System;
using GameModel;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.NetworkSimulator;
using Predassert;

namespace UpdateControls.Correspondence.UnitTest
{
    public class Client
    {
        private Community _community;
        private GameQueue _gameQueue;
        private Person _person;
        private GameRequest _gameRequest;

        public Client(SimulatedNetwork network)
        {
            _community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddCommunicationStrategy(new SimulatedClient(network)
                    .Post<GameQueue>(gameQueue => string.Format("gamequeue/{0}", gameQueue.Identifier))
                    .Get<GameRequest>(
                        () => _person.OutstandingGameRequests,
                        gameRequest => string.Format("gamequeue/{0}/{1}/{2}",
                            gameRequest.GameQueue.Identifier,
                            gameRequest.Person.Unique,
                            gameRequest.Unique
                        )
                    )
                    .Post<Game>(
                        game => UrlOfGame(game)
                    )
                    .Get<Game>(
                        () => _person.UnfinishedGames,
                        game => UrlOfGame(game)
                    )
                );
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _person = _community.AddFact(new Person());
        }

        public Person Person
        {
            get { return _person; }
        }

        public void CreateGameRequest()
        {
            _gameRequest = _gameQueue.CreateGameRequest(_person);
        }

        public void MakeMove(int index)
        {
            _gameRequest.CreateMove(index);
        }

        public void AssertHasGame()
        {
            Pred.Assert(_gameRequest.Game, Is.NotNull<Game>());
        }

        public void AssertHasMove(int index, Guid personId)
        {
            Pred.Assert(_gameRequest.Game.Moves, Contains<Move>.That(
                Has<Move>.Property(move => move.Index, Is.EqualTo(index))
                .And(Has<Move>.Property(move => move.GameRequest.Person.Unique, Is.EqualTo(personId)))));
        }

        private static string UrlOfGame(Game game)
        {
            return string.Format("gamequeue/{0}/{1}/{2}/{3}/{4}",
                game.First.GameQueue.Identifier,
                game.First.Person.Unique,
                game.First.Unique,
                game.Second.Person.Unique,
                game.Second.Unique
            );
        }
    }
}
