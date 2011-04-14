using System.Linq;
using UpdateControls.Fields;
using UpdateControls.Correspondence.POXClient;
using $rootnamespace$.Models;

namespace $rootnamespace$
{
    public class POXConfigurationProvider : IPOXConfigurationProvider
    {
        private Independent<Identity> _identity = new Independent<Identity>();

        public Identity Identity
        {
            get { return _identity; }
            set { _identity.Value = value; }
        }

        public POXConfiguration Configuration
        {
            get
            {
                string address = "https://api.facetedworlds.com:9443/correspondence_server_web/pox";
                string userName = "<<Your username>>";
                string password = "<<Your password>>";
                return new POXConfiguration(address, "$rootnamespace$", userName, password);
            }
        }

        public bool IsToastEnabled
        {
            get { return Identity == null ? false : !Identity.IsToastNotificationDisabled.Any(); }
        }
    }
}
