using System;

namespace UpdateControls.Correspondence.WebServiceClient
{
    public class ServiceClientFactory<TServiceInterface> :
        IServiceClientFactory<TServiceInterface>
        where TServiceInterface : class
    {
        class Client : System.ServiceModel.ClientBase<TServiceInterface>
        {
            public TServiceInterface Service
            {
                get { return base.Channel; }
            }
        }

        public Uri Address
        {
            get
            {
                using (Client client = new Client())
                {
                    return client.Endpoint.Address.Uri;
                }
            }
        }

        public void CallService(Action<TServiceInterface> action)
        {
            Client client = new Client();

            try
            {
                action(client.Service);
                client.Close();
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        public TResult CallService<TResult>(Func<TServiceInterface, TResult> function)
        {
            Client client = new Client();

            try
            {
                TResult result = function(client.Service);
                client.Close();
                return result;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }
    }
}
