using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : ICommunicationStrategy
    {
        private SimulatedNetwork _network;
        private IMessageRepository _repository;

        private List<ServerEndpoint> _postEndpoints = new List<ServerEndpoint>();

        public SimulatedServer(SimulatedNetwork network)
        {
            _network = network;

            network.AttachServer(this);
        }

        public void AttachMessageRepository(IMessageRepository repository)
        {
            _repository = repository;
        }

        public SimulatedServer Post<TFact>(Func<IMessageRepository, string, TFact> urlToFact)
            where TFact : CorrespondenceFact
        {
            _postEndpoints.Add(new ServerEndpoint(typeof(TFact), (repository, url) => urlToFact(repository, url)));
            return this;
        }

        public void Receive(string url, MessageBodyMemento messageBody)
        {
            foreach (ServerEndpoint postEndpoint in _postEndpoints)
            {
                CorrespondenceFact pivot = postEndpoint.GetFact(_repository, url);
                if (pivot != null)
                {
                    IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
                    localIdByRemoteId.Add(messageBody.PivotId, _repository.IDOfFact(pivot));
                    foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
                    {
                        FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                        translatedMemento.Data = identifiedFact.Memento.Data;
                        translatedMemento.AddPredecessors(
                            identifiedFact.Memento.Predecessors
                                .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID]))
                            );
                        CorrespondenceFact fact = _repository.HydrateFact(translatedMemento);
                        fact = _repository.AddFact(fact);
                        FactID localId = _repository.IDOfFact(fact);
                        localIdByRemoteId.Add(identifiedFact.Id, localId);
                    }
                }
            }
        }
    }
}
