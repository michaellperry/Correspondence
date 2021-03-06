﻿using System;
using System.Threading.Tasks;
using Correspondence.Mementos;
using Correspondence.Strategy;
using Correspondence.WorkQueues;

namespace Correspondence.Networking
{
	class PushSubscriptionProxy : IDisposable
	{
		private const long ClientDatabasId = 0;

		private readonly Model _model;
        private readonly AsynchronousServerProxy _serverProxy;
        private readonly CorrespondenceFact _pivot;
        private readonly IWorkQueue _workQueue;
        private IPushSubscription _pushSubscription = null;
  
        public PushSubscriptionProxy(
            Model model,
            AsynchronousServerProxy serverProxy,
            CorrespondenceFact pivot,
            IWorkQueue workQueue)
        {
            _model = model;
            _serverProxy = serverProxy;
            _pivot = pivot;
            _workQueue = workQueue;
        }

		public void Subscribe()
        {
            _workQueue.Perform(() => SubscribeAsync());
        }

		private async Task SubscribeAsync()
		{
            try
            {
                if (_pushSubscription == null)
                {
                    FactTreeMemento pivotTree = new FactTreeMemento(ClientDatabasId);
                    FactID pivotId = _pivot.ID;
                    await _model.AddToFactTreeAsync(pivotTree, pivotId, await _serverProxy.GetPeerId());
                    _pushSubscription = await _serverProxy.CommunicationStrategy.SubscribeForPushAsync(
                        pivotTree,
                        pivotId,
                        await _model.GetClientDatabaseGuidAsync());
                }
            }
            catch (Exception)
            {
                // TODO: Report it.
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
