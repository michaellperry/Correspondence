using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using System;

namespace UpdateControls.Correspondence.Strategy
{
    /// <summary>
	/// </summary>
	public interface IStorageStrategy
    {
        IDisposable BeginUnitOfWork();

        // Facts.
        bool GetID(string factName, out FactID id);
        void SetID(string factName, FactID id);
        FactMemento Load(FactID id);
        bool Save(FactMemento memento, List<FactID> pivots, out FactID id);
        bool FindExistingFact(FactMemento memento, out FactID id);
        IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId);
        IEnumerable<IdentifiedFactMemento> LoadAllFacts();

        // Messages.
        TimestampID LoadTimestamp(string protocolName, string peerName);
        void SaveTimestamp(string protocolName, string peerName, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessages(ref TimestampID timestamp);

        // Networking.
        int SavePeer(string protocolName, string peerName);
        FactID GetFactIDFromShare(int peerId, FactID remoteFactId);
        void SaveShare(int peerId, FactID remoteFactId, FactID localFactId);
        IEnumerable<NamedFactMemento> LoadAllNamedFacts();
    }
}
