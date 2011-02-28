using System.Collections.Generic;

namespace UpdateControls.Correspondence.Networking
{
	internal interface ISubscriptionProvider
	{
		IEnumerable<Subscription> Subscriptions { get; }
	}
}
