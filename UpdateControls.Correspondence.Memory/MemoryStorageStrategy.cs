﻿using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class MemoryStorageStrategy : IStorageStrategy
    {
        private List<IdentifiedFactMemento> _factTable = new List<IdentifiedFactMemento>();
        private List<MessageMemento> _messageTable = new List<MessageMemento>();
        private IDictionary<PeerIdentifier, TimestampID> _timestampByPeer = new Dictionary<PeerIdentifier,TimestampID>();

        public IDisposable BeginUnitOfWork()
        {
            return new UnitOfWork();
        }

        public bool GetID(string factName, out FactID id)
        {
            throw new NotImplementedException();
        }

        public void SetID(string factName, FactID id)
        {
            throw new NotImplementedException();
        }

        public FactMemento Load(FactID id)
        {
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Id.Equals(id));
            if (fact != null)
                return fact.Memento;
            else
                throw new CorrespondenceException(string.Format("Fact with id {0} not found.", id));
        }

        public bool Save(FactMemento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
            if (fact == null)
            {
                // It doesn't, so create it.
                id = new FactID() { key = _factTable.Count };
                fact = new IdentifiedFactMemento(id, memento);
                _factTable.Add(fact);
                return true;
            }
            else
            {
                id = fact.Id;
                return false;
            }
        }

        public bool FindExistingFact(FactMemento memento, out FactID id)
        {
            // See if the fact already exists.
            IdentifiedFactMemento fact = _factTable.FirstOrDefault(o => o.Memento.Equals(memento));
            if (fact == null)
            {
                id = new FactID();
                return false;
            }
            else
            {
                id = fact.Id;
                return true;
            }
        }

        public IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            return new QueryExecutor(_factTable).ExecuteQuery(queryDefinition, startingId, options).Reverse();
        }

        public IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId)
        {
            return QueryForFacts(queryDefinition, startingId, null).Select(im => im.Id);
        }


        public int SavePeer(string protocolName, string peerName)
        {
            throw new NotImplementedException();
        }

        public TimestampID LoadTimestamp(string protocolName, string peerName)
        {
            TimestampID timestamp;
            if (_timestampByPeer.TryGetValue(new PeerIdentifier(protocolName, peerName), out timestamp))
                return timestamp;
            else
                return new TimestampID();
        }

        public void SaveTimestamp(string protocolName, string peerName, TimestampID timestamp)
        {
            _timestampByPeer[new PeerIdentifier(protocolName, peerName)] = timestamp;
        }

        public IEnumerable<MessageMemento> LoadRecentMessages(ref TimestampID timestamp)
        {
            TimestampID startingTimestamp = timestamp;
            List<MessageMemento> messages = _messageTable
                .Where(message => message.FactId.key > startingTimestamp.key)
                .ToList();
            if (messages.Any())
            {
                long maxKey = messages.Max(message => message.FactId.key);
                timestamp = new TimestampID() { key = maxKey };
            }
            return messages;
        }

        public IEnumerable<IdentifiedFactMemento> LoadAllFacts()
        {
            throw new NotImplementedException();
        }

        public FactID GetFactIDFromShare(int peerId, FactID remoteFactId)
        {
            throw new NotImplementedException();
        }

        public void SaveShare(int peerId, FactID remoteFactId, FactID localFactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NamedFactMemento> LoadAllNamedFacts()
        {
            throw new NotImplementedException();
        }
    }
}
