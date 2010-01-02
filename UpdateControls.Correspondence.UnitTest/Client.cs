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
                    .Get<GameRequest>(
                        () => _person.OutstandingGameRequests
                    )
                    .Get<Game>(
                        () => _person.UnfinishedGames
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

        public Move MakeMove(int index)
        {
            return _gameRequest.Game.CreateMove(_person, index);
        }

        public void AssertHasGame()
        {
            Pred.Assert(_gameRequest.Game, Is.NotNull<Game>());
        }

        public void AssertHasMove(int index, Guid personId)
        {
            AssertHasMove(index, personId, 0);
        }

        public void AssertHasMove(int index, Guid personId, int result)
        {
            Pred.Assert(_gameRequest.Game.Moves, Contains<Move>.That(
                Has<Move>.Property(move => move.Index, Is.EqualTo(index))
                .And(Has<Move>.Property(move => move.Person.Unique, Is.EqualTo(personId)))
                .And(Has<Move>.Property(move => move.Result, Is.EqualTo(result)))
                ));
        }
    }
}
