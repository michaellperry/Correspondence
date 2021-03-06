using System;
using System.Collections.Generic;
using Correspondence.Mementos;
using Correspondence.Queries;
using Correspondence.Tasks;

namespace Correspondence.Strategy
{
    /// <summary>
	/// </summary>
	public interface IStorageStrategy
    {
        IDisposable BeginDuration();
		Guid ClientGuid { get; }

		// Facts.
        bool GetID(string factName, out FactID id);
        void SetID(string factName, FactID id);
        FactMemento Load(FactID id);
        bool Save(FactMemento memento, int peerId, out FactID id);
        Task<FactID?> FindExistingFactAsync(FactMemento memento);
        Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId);
        IEnumerable<IdentifiedFactMemento> LoadAllFacts();
        IdentifiedFactMemento LoadNextFact(FactID? lastFactId);

        // Messages.
        TimestampID LoadOutgoingTimestamp(int peerId);
        void SaveOutgoingTimestamp(int peerId, TimestampID timestamp);
        TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId);
        void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp);
        IEnumerable<FactID> LoadRecentMessagesForClient(FactID pivotId, TimestampID timestamp);
        void Unpublish(FactID factId, RoleMemento role);

        // Networking.
        int SavePeer(string protocolName, string peerName);
        FactID GetFactIDFromShare(int peerId, FactID remoteFactId);
        bool GetRemoteId(FactID localFactId, int peerId, out FactID remoteFactId);
        void SaveShare(int peerId, FactID remoteFactId, FactID localFactId);
        IEnumerable<NamedFactMemento> LoadAllNamedFacts();
    }
}
