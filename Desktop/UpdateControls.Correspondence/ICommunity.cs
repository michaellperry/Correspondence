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
        void BeginSynchronize(AsyncCallback callback, object state);
        bool EndSynchronize(IAsyncResult asyncResult);
        bool Synchronizing { get; }
        Exception LastException { get; }
    }
}
