using System;
using GameModel;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.NetworkSimulator;

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
                );
            _gameQueue = _community.AddFact(new GameQueue("mygamequeue"));
            _person = _community.AddFact(new Person());
        }

        public void CreateGameRequest()
        {
            _gameRequest = _gameQueue.CreateGameRequest(_person);
        }
    }
}
