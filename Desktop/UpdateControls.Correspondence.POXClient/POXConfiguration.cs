
namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
		private string _endpointBase;

		public POXConfiguration(string endpointBase)
		{
			_endpointBase = endpointBase;
		}

		public string EndpointBase
		{
			get { return _endpointBase; }
		}
	}
}
