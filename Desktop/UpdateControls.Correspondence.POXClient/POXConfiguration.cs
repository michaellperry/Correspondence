
namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
		private string _endpoint;

		public POXConfiguration(string endpoint)
		{
			_endpoint = endpoint;
		}

		public string Endpoint
		{
			get { return _endpoint; }
		}
	}
}
