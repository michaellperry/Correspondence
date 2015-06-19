using System.Collections.Generic;

namespace Correspondence.Networking
{
	internal interface ISubscriptionProvider
	{
		IEnumerable<Subscription> Subscriptions { get; }
	}
}
