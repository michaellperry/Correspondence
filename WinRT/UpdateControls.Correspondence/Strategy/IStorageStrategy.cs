using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using System;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.Strategy
{
    /// <summary>
	/// </summary>
	public interface IStorageStrategy
    {
		Guid ClientGuid { get; }

		// Facts.
        bool GetID(string factName, out FactID id);
        void SetID(string factName, FactID id);
        FactMemento Load(FactID id);
        Task<SaveResult> SaveAsync(FactMemento memento, int peerId);
        bool FindExistingFact(FactMemento memento, out FactID id);
        Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        Task<IEnumerable<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId);

        // Messages.
        TimestampID LoadOutgoingTimestamp(int peerId);
        void SaveOutgoingTimestamp(int peerId, TimestampID timestamp);
        TimestampID LoadIncomingTimestamp(int peerId, FactID pivotId);
        void SaveIncomingTimestamp(int peerId, FactID pivotId, TimestampID timestamp);
        IEnumerable<MessageMemento> LoadRecentMessagesForServer(int peerId, TimestampID timestamp);

        // Networking.
        int SavePeer(string protocolName, string peerName);
        FactID GetFactIDFromShare(int peerId, FactID remoteFactId);
        bool GetRemoteId(FactID localFactId, int peerId, out FactID remoteFactId);
        void SaveShare(int peerId, FactID remoteFactId, FactID localFactId);
    }
}
