using System;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence
{
    public interface ICommunity
    {
        Task<T> AddFactAsync<T>(T prototype)
            where T : CorrespondenceFact;
        Task<T> FindFactAsync<T>(T prototype)
            where T : CorrespondenceFact;
        event Action<CorrespondenceFact> FactAdded;
        Task<bool> SynchronizeAsync();
        void BeginSending();
        void BeginReceiving();
        bool Synchronizing { get; }
        Exception LastException { get; }
    }
}
