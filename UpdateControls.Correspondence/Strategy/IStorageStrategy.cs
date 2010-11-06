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
        IDisposable BeginDuration();

        // Facts.
        bool GetID(string factName, out FactID id);
        void SetID(string factName, FactID id);
        FactMemento Load(FactID id);
        bool Save(FactMemento memento, string protocolName, string peerName, out FactID id);
        bool FindExistingFact(FactMemento memento, out FactID id);
        IEnumerable<IdentifiedFactMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId);
        IEnumerable<IdentifiedFactMemento> LoadAllFacts();

        // Messages.
        TimestampID LoadOutgoingTimestamp(string protocolName, string peerName);
        void SaveOutgoingTimestamp(string protocolName, string peerName, TimestampID timestamp);
        TimestampID LoadIncomingTimestamp(string protocolName, string peerName, FactID pivotId);
        void SaveIncomingTimestamp(string protocolName, string peerName, FactID pivotId, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessagesForServer(TimestampID timestamp, string protocolName, string peerName);
        IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp);

        // Networking.
        int SavePeer(string protocolName, string peerName);
        FactID GetFactIDFromShare(int peerId, FactID remoteFactId);
        void SaveShare(int peerId, FactID remoteFactId, FactID localFactId);
        IEnumerable<NamedFactMemento> LoadAllNamedFacts();
    }
}
