using System.Configuration;
using System.Linq;
using UpdateControls.Fields;
using UpdateControls.Correspondence.BinaryHTTPClient;

namespace $rootnamespace$
{
    public class HTTPConfigurationProvider : IHTTPConfigurationProvider
    {
        public HTTPConfiguration Configuration
        {
            get
            {
                string address = ConfigurationManager.AppSettings["CorrespondenceAddress"];
                string apiKey = ConfigurationManager.AppSettings["CorrespondenceAPIKey"];
                int timeoutSeconds = int.Parse(ConfigurationManager.AppSettings["CorrespondencePollingIntervalSeconds"]);
                return new HTTPConfiguration(address, "$rootnamespace$", apiKey, timeoutSeconds);
            }
        }
    }
}
