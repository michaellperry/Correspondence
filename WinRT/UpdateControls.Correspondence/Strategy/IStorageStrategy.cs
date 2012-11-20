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
        Task<FactMemento> LoadAsync(FactID id);
        Task<SaveResult> SaveAsync(FactMemento memento, int peerId);
        Task<FactID?> FindExistingFactAsync(FactMemento memento);
        Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        Task<IEnumerable<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId);

        // Messages.
        Task<TimestampID> LoadOutgoingTimestampAsync(int peerId);
        Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp);
        Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId);
        Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp);
        Task<IEnumerable<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp);

        // Networking.
        Task<int> SavePeerAsync(string protocolName, string peerName);
        Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId);
        Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId);
        Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId);
    }
}
