using UpdateControls.Correspondence.POXClient;

namespace Reversi.Client
{
    public class POXClientConfigurationProvider : IPOXConfigurationProvider
    {
        public POXConfiguration Configuration
        {
            get { return new POXConfiguration("http://localhost:14112/correspondence_server_web/pox/"); }
        }
    }
}
