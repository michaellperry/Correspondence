using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class MemoryCommunicationStrategy : ICommunicationStrategy
    {
        private MemoryStorageStrategy _repository = new MemoryStorageStrategy();
        private long _databaseId = 0L;

        public string ProtocolName
        {
            get { return "Memory"; }
        }

        public string PeerName
        {
            get { return "Local"; }
        }

        public GetResultMemento Get(FactTreeMemento pivotTree, FactID remotePivotId, TimestampID timestamp)
        {
            FactID? localPivotId = FindExistingFact(remotePivotId, pivotTree);
            if (!localPivotId.HasValue)
                return new GetResultMemento(new FactTreeMemento(_databaseId), new TimestampID(_databaseId, timestamp.Key));

            IEnumerable<FactID> recentMessages = _repository.LoadRecentMessagesForClient(localPivotId.Value, timestamp);
            long nextTimestamp = recentMessages.Any() ? recentMessages.Max(message => message.key) : timestamp.Key;
            FactTreeMemento messageBody = new FactTreeMemento(_databaseId);
            foreach (FactID recentMessage in recentMessages)
                AddToFactTree(messageBody, recentMessage);

            return new GetResultMemento(messageBody, new TimestampID(_databaseId, nextTimestamp));
        }

        public void Post(FactTreeMemento messageBody, List<UnpublishMemento> unpublishedMessages)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactBase identifiedFact in messageBody.Facts)
            {
                FactID localId;
                if (identifiedFact is IdentifiedFactMemento)
                {
                    FactMemento memento = ((IdentifiedFactMemento)identifiedFact).Memento;
                    FactMemento translatedMemento = new FactMemento(memento.FactType);
                    translatedMemento.Data = memento.Data;
                    translatedMemento.AddPredecessors(memento.Predecessors
                        .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID], remote.IsPivot)));
                    localId = _repository.SaveAsync(translatedMemento, 0).Result.Id;
                }
                else
                {
                    // I am remote to the sender, so my local ID is his remote ID.
                    var identifiedFactRemote = (IdentifiedFactRemote)identifiedFact;
                    localId = identifiedFactRemote.RemoteId;
                }
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }

            foreach (UnpublishMemento unpublishedMessage in unpublishedMessages)
            {
                FactID localMessageId;
                if (localIdByRemoteId.TryGetValue(unpublishedMessage.MessageId, out localMessageId))
                    _repository.Unpublish(localMessageId, unpublishedMessage.Role);
            }
        }

        private FactID? FindExistingFact(FactID remotePivotId, FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactBase identifiedFact in messageBody.Facts)
            {
                FactID localId;
                if (identifiedFact is IdentifiedFactMemento)
                {
                    FactMemento memento = ((IdentifiedFactMemento)identifiedFact).Memento;
                    FactMemento translatedMemento = new FactMemento(memento.FactType);
                    translatedMemento.Data = memento.Data;
                    translatedMemento.AddPredecessors(memento.Predecessors
                        .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID], remote.IsPivot)));
                    if (!_repository.FindExistingFact(translatedMemento, out localId))
                        return null;
                }
                else
                {
                    // I am remote to the sender, so my local ID is his remote ID.
                    var identifiedFactRemote = (IdentifiedFactRemote)identifiedFact;
                    localId = identifiedFactRemote.RemoteId;
                }
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
