using System;

namespace UpdateControls.Correspondence.Async
{
	internal class ResultAggregate
	{
		private Action _callback;
		private int _waitCount = 0;
		private bool _closed = false;

		public ResultAggregate(Action callback)
		{
			_callback = callback;
		}

		public void Begin()
		{
			lock (this)
			{
				++_waitCount;
			}
		}

		public void End()
		{
			lock (this)
			{
				--_waitCount;
				CheckForCompletion();
			}
		}

		public void Close()
		{
			lock (this)
			{
				_closed = true;
				CheckForCompletion();
			}
		}

		private void CheckForCompletion()
		{
			if (_closed && _waitCount == 0)
				_callback();
		}
	}
}
