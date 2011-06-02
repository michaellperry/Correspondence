using System;

namespace UpdateControls.Correspondence
{
    public interface ICommunity
    {
        IDisposable BeginDuration();
        T AddFact<T>(T prototype)
            where T : CorrespondenceFact;
        T FindFact<T>(T prototype)
            where T : CorrespondenceFact;
        event Action FactAdded;
        bool Synchronize();
        void BeginSynchronizeIncoming();
        void BeginSynchronizeOutgoing();
        bool Synchronizing { get; }
        Exception LastException { get; }
    }
}
