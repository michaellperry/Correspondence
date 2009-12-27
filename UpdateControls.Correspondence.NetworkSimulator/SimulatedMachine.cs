using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedMachine : ICommunicationStrategy
    {
        protected IMessageRepository _repository;

        public void AttachMessageRepository(IMessageRepository repository)
        {
            _repository = repository;
        }

        protected void AddFactTree(MessageBodyMemento messageBody, FactID factId)
        {
            if (!factId.Equals(messageBody.PivotId) && !messageBody.Contains(factId))
            {
                FactMemento fact = _repository.LoadFact(factId);
                foreach (PredecessorMemento predecessor in fact.Predecessors)
                    AddFactTree(messageBody, predecessor.ID);
                messageBody.Add(new IdentifiedFactMemento(factId, fact));
            }
        }

        protected TimestampID ReceiveMessage(MessageBodyMemento messageBody, CorrespondenceFact pivot)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            localIdByRemoteId.Add(messageBody.PivotId, _repository.IDOfFact(pivot));
            long maxRemoteId = messageBody.PivotId.key;
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors.Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId = _repository.SaveFact(translatedMemento);
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);

                if (remoteId.key > maxRemoteId)
                    maxRemoteId = remoteId.key;
            }
            return new TimestampID() { key = maxRemoteId };
        }
    }
}
