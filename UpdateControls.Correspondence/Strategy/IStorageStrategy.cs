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
        Memento Load(FactID id);
        bool Save(Memento memento, out FactID id);
        bool FindExistingFact(Memento memento, out FactID id);
        IEnumerable<IdentifiedMemento> QueryForFacts(QueryDefinition queryDefinition, FactID startingId, QueryOptions options);
        IEnumerable<FactID> QueryForIds(QueryDefinition queryDefinition, FactID startingId);
        IEnumerable<IdentifiedMemento> LoadAllFacts();

        // Networking.
        int SavePeer(string protocolName, string peerName);
        FactID GetFactIDFromShare(int peerId, FactID remoteFactId);
        void SaveShare(int peerId, FactID remoteFactId, FactID localFactId);
        IEnumerable<NamedFactMemento> LoadAllNamedFacts();
    }
}
