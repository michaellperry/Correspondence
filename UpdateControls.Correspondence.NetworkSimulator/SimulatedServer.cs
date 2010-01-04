using System;
using System.Linq;
using System.Collections.Generic;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : ICommunicationStrategy
    {
        private IStorageStrategy _storageStrategy;
        private List<RoleMemento> _pivotRoles = new List<RoleMemento>();

        public SimulatedServer(IStorageStrategy storageStrategy)
        {
            _storageStrategy = storageStrategy;
        }

        public SimulatedServer AddPivot(RoleMemento role)
        {
            _pivotRoles.Add(role);
            return this;
        }

        public string ProtocolName
        {
            get { return "simulation"; }
        }

        public string PeerName
        {
            get { return "server"; }
        }

        public void Post(FactTreeMemento messageBody)
        {
            ReceiveMessage(messageBody);
        }

        public FactTreeMemento Get(FactTreeMemento rootTree, FactID remoteRootId, TimestampID timestamp)
        {
            FactID localRootId = FindExistingFact(remoteRootId, rootTree);
            IEnumerable<FactID> recentMessages = _storageStrategy.LoadRecentMessages(localRootId, timestamp);
            FactTreeMemento messageBody = new FactTreeMemento();
            foreach (FactID recentMessage in recentMessages)
                AddToFactTree(messageBody, recentMessage);

            return messageBody;
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
                    throw new NetworkSimulatorException("The message does not refer to an existing fact.");
                FactID remoteId = identifiedFact.Id;
                localIdByRemoteId.Add(remoteId, localId);
            }
            return localIdByRemoteId[remoteRootId];
        }

        private IEnumerable<FactID> FindPivots(FactMemento memento)
        {
            return memento.Predecessors
                .Where(predecessor => _pivotRoles.Contains(predecessor.Role))
                .Select(predecessor => predecessor.ID);
        }
    }
}
