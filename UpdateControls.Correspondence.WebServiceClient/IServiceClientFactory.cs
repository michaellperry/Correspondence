using System;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public interface IServiceClientFactory<TServiceInterface>
    {
        void CallService(Action<TServiceInterface> action);
        TResult CallService<TResult>(Func<TServiceInterface, TResult> function);
    }
}
