using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class MemoryCommunicationStrategy : ICommunicationStrategy
    {
        private IStorageStrategy _repository = new MemoryStorageStrategy();
        private long _databaseId = 0L;

        public string ProtocolName
        {
            get { return "Memory"; }
        }

        public string PeerName
        {
            get { return "Local"; }
        }

        public FactTreeMemento Get(FactTreeMemento pivotTree, FactID remotePivotId, TimestampID timestamp)
        {
            FactID? localPivotId = FindExistingFact(remotePivotId, pivotTree);
            if (!localPivotId.HasValue)
                return new FactTreeMemento(_databaseId, timestamp.Key);

            IEnumerable<FactID> recentMessages = _repository.LoadRecentMessagesForClient(localPivotId.Value, timestamp);
            long nextTimestamp = recentMessages.Any() ? recentMessages.Max(message => message.key) : timestamp.Key;
            FactTreeMemento messageBody = new FactTreeMemento(_databaseId, nextTimestamp);
            foreach (FactID recentMessage in recentMessages)
                AddToFactTree(messageBody, recentMessage);

            return messageBody;
        }

        public void Post(FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId;
                _repository.Save(translatedMemento, string.Empty, string.Empty, out localId);
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }
        }

        private FactID? FindExistingFact(FactID remotePivotId, FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId;
                if (!_repository.FindExistingFact(translatedMemento, out localId))
                    return null;
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }
            return localIdByRemoteId[remotePivotId];
        }

        private void AddToFactTree(FactTreeMemento messageBody, FactID factId)
        {
            if (!messageBody.Contains(factId))
            {
                FactMemento fact = _repository.Load(factId);
                foreach (PredecessorMemento predecessor in fact.Predecessors)
                    AddToFactTree(messageBody, predecessor.ID);
                messageBody.Add(new IdentifiedFactMemento(factId, fact));
            }
        }
    }
}
