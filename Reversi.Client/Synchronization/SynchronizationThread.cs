using System;
using System.Threading;
using UpdateControls;
using UpdateControls.Correspondence;

namespace Reversi.Client.Synchronization
{
    public class SynchronizationThread
    {
        private Community _community;

        private string _lastError;
        private Independent _indLastError = new Independent();

        public SynchronizationThread(Community community)
        {
            _community = community;
        }

        public void Start()
        {
            Wake();
        }

        public void Stop()
        {
        }

        public void Wake()
        {
            _community.BeginSynchronize(SynchronizeCompleted, null);
        }

        public string LastError
        {
            get
            {
                lock (this)
                {
                    _indLastError.OnGet();
                    return _lastError;
                }
            }

            private set
            {
                lock (this)
                {
                    _indLastError.OnSet();
                    _lastError = value;
                }
            }
        }

        private void SynchronizeCompleted(IAsyncResult a)
        {
            try
            {
                bool synchronized = _community.EndSynchronize(a);
                LastError = null;
                if (synchronized)
                    Wake();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }
    }
}
