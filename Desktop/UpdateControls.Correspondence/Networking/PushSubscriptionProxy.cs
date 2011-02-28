using System;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Networking
{
	class PushSubscriptionProxy : IDisposable
	{
		private const long ClientDatabasId = 0;

		private Model _model;
		private IAsynchronousCommunicationStrategy _strategy;
		private CorrespondenceFact _pivot;
		private IPushSubscription _pushSubscription = null;

		public PushSubscriptionProxy(Model model, IAsynchronousCommunicationStrategy strategy, CorrespondenceFact pivot)
		{
			_model = model;
			_strategy = strategy;
			_pivot = pivot;
		}

		public void Subscribe()
		{
			if (_pushSubscription == null)
			{
				FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId);
				FactID pivotId = _pivot.ID;
				_model.AddToFactTree(pivotTree, pivotId);
				_pushSubscription = _strategy.SubscribeForPush(pivotTree, pivotId, _model.ClientDatabaseGuid);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			PushSubscriptionProxy that = obj as PushSubscriptionProxy;
			if (that == null)
				return false;
			return this._strategy == that._strategy && this._pivot == that._pivot;
		}

		public override int GetHashCode()
		{
			return _strategy.GetHashCode() * 37 + _pivot.GetHashCode();
		}

		public void Dispose()
		{
			if (_pushSubscription != null)
				_pushSubscription.Unsubscribe();
			_pushSubscription = null;
		}
	}
}
