using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.WebService.Contract;

namespace UpdateControls.Correspondence.WebService
{
    public class SynchronizationService : ISynchronizationService
    {
        private static IStorageStrategy _storageStrategy = new MemoryStorageStrategy();
        private static List<RoleMemento> _pivotRoles = new List<RoleMemento>()
        {
            new RoleMemento(
                new CorrespondenceFactType("GameModel.GameRequest", 1),
                "gameQueue",
                new CorrespondenceFactType("GameModel.GameQueue", 1)
            ),
            new RoleMemento(
                new CorrespondenceFactType("GameModel.Game", 1),
                "gameRequest",
                new CorrespondenceFactType("GameModel.GameRequest", 1)
            ),
            new RoleMemento(
                new CorrespondenceFactType("GameModel.Move", 1),
                "game",
                new CorrespondenceFactType("GameModel.Game", 1)
            )
        };

        public FactTree Get(FactTree rootTree, long rootId, long timestamp)
        {
            FactTreeMemento result = Get(
                Translate.FactTreeToMemento(rootTree),
                Translate.LongToFactID(rootId),
                Translate.LongToTimestampID(timestamp));
            return Translate.MementoToFactTree(result);
        }

        public void Post(FactTree messageBody)
        {
            ReceiveMessage(Translate.FactTreeToMemento(messageBody));
        }

        private FactTreeMemento Get(FactTreeMemento rootTree, FactID remoteRootId, TimestampID timestamp)
        {
            FactID localRootId = FindExistingFact(remoteRootId, rootTree);
            IEnumerable<FactID> recentMessages = _storageStrategy.LoadRecentMessages(localRootId, timestamp);
            FactTreeMemento messageBody = new FactTreeMemento();
            foreach (FactID recentMessage in recentMessages)
                AddToFactTree(messageBody, recentMessage);

            return messageBody;
        }

        private TimestampID ReceiveMessage(FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            long maxRemoteId = 0;
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId;
                _storageStrategy.Save(translatedMemento, FindPivots(translatedMemento).ToList(), out localId);
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);

                if (remoteId.key > maxRemoteId)
                    maxRemoteId = remoteId.key;
            }
            return new TimestampID() { key = maxRemoteId };
        }

        private FactID FindExistingFact(FactID remoteRootId, FactTreeMemento messageBody)
        {
            IDictionary<FactID, FactID> localIdByRemoteId = new Dictionary<FactID, FactID>();
            foreach (IdentifiedFactMemento identifiedFact in messageBody.Facts)
            {
                FactMemento translatedMemento = new FactMemento(identifiedFact.Memento.FactType);
                translatedMemento.Data = identifiedFact.Memento.Data;
                translatedMemento.AddPredecessors(identifiedFact.Memento.Predecessors
                    .Select(remote => new PredecessorMemento(remote.Role, localIdByRemoteId[remote.ID])));
                FactID localId;
                if (!_storageStrategy.FindExistingFact(translatedMemento, out localId))
                    throw new WebServiceException("The message does not refer to an existing fact.");
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }
            return localIdByRemoteId[remoteRootId];
        }

        private void AddToFactTree(FactTreeMemento messageBody, FactID factId)
        {
            if (!messageBody.Contains(factId))
            {
                FactMemento fact = _storageStrategy.Load(factId);
                foreach (PredecessorMemento predecessor in fact.Predecessors)
                    AddToFactTree(messageBody, predecessor.ID);
                messageBody.Add(new IdentifiedFactMemento(factId, fact));
            }
        }

        private IEnumerable<FactID> FindPivots(FactMemento memento)
        {
            return memento.Predecessors
                .Where(predecessor => _pivotRoles.Contains(predecessor.Role))
                .Select(predecessor => predecessor.ID);
        }
    }
}
