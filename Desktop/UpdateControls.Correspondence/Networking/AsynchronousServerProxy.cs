using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
	internal class AsynchronousServerProxy
	{
		public IAsynchronousCommunicationStrategy CommunicationStrategy;
		public int PeerId;
	}
}
