using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.POXClient
{
	public class NoOpPushSubscription : IPushSubscription
	{
		public void Unsubscribe()
		{
		}
	}
}
