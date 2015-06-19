using System.Linq;
using Assisticant.Fields;
using Correspondence.BinaryHTTPClient;

namespace $rootnamespace$
{
    public class HTTPConfigurationProvider : IHTTPConfigurationProvider
    {
        private Observable<Individual> _individual = new Observable<Individual>();

        public Individual Individual
        {
            get { return _individual; }
            set { _individual.Value = value; }
        }

        public HTTPConfiguration Configuration
        {
            get
            {
                string address = "http://correspondencedistributor.azurewebsites.net/";
                string apiKey = "<<Your API key>>";
                return new HTTPConfiguration(address, "$rootnamespace$", apiKey);
            }
        }

        public bool IsToastEnabled
        {
            get { return false; }
        }
    }
}
