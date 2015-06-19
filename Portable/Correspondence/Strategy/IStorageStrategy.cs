using System.Collections.Generic;
using Correspondence.Mementos;
using Correspondence.Queries;
using System;
using System.Threading.Tasks;

namespace Correspondence.Strategy
{
    /// <summary>
	/// </summary>
	public interface IStorageStrategy
    {
        // Capabilities.
        bool IsSynchronous { get; }

        Task<Guid> GetClientGuidAsync();

		// Facts.
        Task<FactID?> GetIDAsync(string factName);
        Task SetIDAsync(string factName, FactID id);
        Task<FactMemento> LoadAsync(FactID id);
        Task<SaveResult> SaveAsync(FactMemento memento, int peerId);
        Task<FactID?> FindExistingFactAsync(FactMemento memento);
        Task<List<IdentifiedFactMemento>> QueryForFactsAsync(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        Task<List<FactID>> QueryForIdsAsync(QueryDefinition queryDefinition, FactID startingId);

        // Messages.
        Task<TimestampID> LoadOutgoingTimestampAsync(int peerId);
        Task SaveOutgoingTimestampAsync(int peerId, TimestampID timestamp);
        Task<TimestampID> LoadIncomingTimestampAsync(int peerId, FactID pivotId);
        Task SaveIncomingTimestampAsync(int peerId, FactID pivotId, TimestampID timestamp);
        Task<List<MessageMemento>> LoadRecentMessagesForServerAsync(int peerId, TimestampID timestamp);

        // Networking.
        Task<int> SavePeerAsync(string protocolName, string peerName);
        Task<FactID> GetFactIDFromShareAsync(int peerId, FactID remoteFactId);
        Task<FactID?> GetRemoteIdAsync(FactID localFactId, int peerId);
        Task SaveShareAsync(int peerId, FactID remoteFactId, FactID localFactId);

        // Debugging.
        List<CorrespondenceFactType> GetAllTypes();
        List<IdentifiedFactMemento> GetPageOfFactsForType(CorrespondenceFactType type, int page);
        List<IdentifiedFactMemento> GetAllSuccessors(FactID factId);
    }
}
