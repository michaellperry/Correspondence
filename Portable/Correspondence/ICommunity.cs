using System;
using System.Threading.Tasks;

namespace Correspondence
{
    public interface ICommunity
    {
        Task<T> AddFactAsync<T>(T prototype)
            where T : CorrespondenceFact;
        T FindFact<T>(T prototype)
            where T : CorrespondenceFact;
        event Action<CorrespondenceFact> FactAdded;
        Task<bool> SynchronizeAsync();
        void BeginSending();
        void BeginReceiving();
        bool Synchronizing { get; }
        Exception LastException { get; }
        void Perform(Func<Task> asyncDelegate);
    }
}
