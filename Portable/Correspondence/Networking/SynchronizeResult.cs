using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Correspondence.Networking
{
	internal class SynchronizeResult : IAsyncResult
	{
		private AsyncCallback _callback;
		private object _state;
		private bool? _outgoingResult = null;
		private bool? _incomingResult = null;

		public SynchronizeResult(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;
		}

		public void OutgoingFinished(bool any)
		{
			_outgoingResult = any;
			Finish();
		}

		public void IncomingFinished(bool any)
		{
			_incomingResult = any;
			Finish();
		}

		private void Finish()
		{
			if (_outgoingResult != null && _incomingResult != null)
			{
				_callback(this);
			}
		}

		public bool? OutgoingResult
		{
			get { return _outgoingResult; }
		}

		public bool? IncomingResult
		{
			get { return _incomingResult; }
		}

		#region IAsyncResult Members

		public object AsyncState
		{
			get { throw new NotImplementedException(); }
		}

		public System.Threading.WaitHandle AsyncWaitHandle
		{
			get { throw new NotImplementedException(); }
		}

		public bool CompletedSynchronously
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsCompleted
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
