using System;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public interface IServiceClientFactory<TServiceInterface>
    {
        Uri Address { get; }
        void CallService(Action<TServiceInterface> action);
        TResult CallService<TResult>(Func<TServiceInterface, TResult> function);
    }
}
