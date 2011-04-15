using System.Linq;
using UpdateControls.Fields;
using UpdateControls.Correspondence.POXClient;
using $rootnamespace$.Models;

namespace $rootnamespace$
{
    public class POXConfigurationProvider : IPOXConfigurationProvider
    {
        public POXConfiguration Configuration
        {
            get
            {
                string address = "https://api.facetedworlds.com:9443/correspondence_server_web/pox";
                string apiKei = "<<Your API key>>";
                return new POXConfiguration(address, "$rootnamespace$", apiKey);
            }
        }
    }
}
