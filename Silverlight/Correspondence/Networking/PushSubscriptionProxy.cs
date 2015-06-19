using System;
using Correspondence.Mementos;
using Correspondence.Strategy;

namespace Correspondence.Networking
{
	class PushSubscriptionProxy : IDisposable
	{
		private const long ClientDatabasId = 0;

		private readonly Model _model;
        private readonly AsynchronousServerProxy _serverProxy;
        private readonly CorrespondenceFact _pivot;
		private IPushSubscription _pushSubscription = null;
  
        public PushSubscriptionProxy(Model model, AsynchronousServerProxy serverProxy, CorrespondenceFact pivot)
        {
            _model = model;
            _serverProxy = serverProxy;
            _pivot = pivot;
        }

		public void Subscribe()
		{
			if (_pushSubscription == null)
			{
				FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId);
				FactID pivotId = _pivot.ID;
				_model.AddToFactTree(pivotTree, pivotId, _serverProxy.PeerId);
				_pushSubscription = _serverProxy.CommunicationStrategy.SubscribeForPush(pivotTree, pivotId, _model.ClientDatabaseGuid);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			PushSubscriptionProxy that = obj as PushSubscriptionProxy;
			if (that == null)
				return false;
			return this._serverProxy == that._serverProxy && this._pivot == that._pivot;
		}

		public override int GetHashCode()
		{
			return _serverProxy.GetHashCode() * 37 + _pivot.GetHashCode();
		}

		public void Dispose()
		{
			if (_pushSubscription != null)
				_pushSubscription.Unsubscribe();
			_pushSubscription = null;
		}
	}
}
