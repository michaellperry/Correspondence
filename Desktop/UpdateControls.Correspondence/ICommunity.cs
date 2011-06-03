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
        event Action<CorrespondenceFact> FactAdded;
        bool Synchronize();
        void BeginSending();
        void BeginReceiving();
        bool Synchronizing { get; }
        Exception LastException { get; }
    }
}
