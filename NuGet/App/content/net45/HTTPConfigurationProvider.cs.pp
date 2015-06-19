using System.Linq;
using Assisticant.Fields;
using Correspondence.BinaryHTTPClient;

namespace $rootnamespace$
{
    public class HTTPConfigurationProvider : IHTTPConfigurationProvider
    {
        public HTTPConfiguration Configuration
        {
            get
            {
                string address = "http://correspondencedistributor.azurewebsites.net/";
                string apiKey = "<<Your API key>>";
				int timeoutSeconds = 30;
                return new HTTPConfiguration(address, "$rootnamespace$", apiKey, timeoutSeconds);
            }
        }
    }
}
