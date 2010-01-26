using System;
using System.Threading;
using UpdateControls;
using UpdateControls.Correspondence;

namespace Reversi.Client.Synchronization
{
    public class SynchronizationThread
    {
        private Community _community;

        private Thread _thread;
        private ManualResetEvent _stopping;

        private string _lastError;
        private Independent _indLastError = new Independent();

        public SynchronizationThread(Community community)
        {
            _community = community;
            _thread = new Thread(SynchronizeProc);
            _thread.Name = "Correspondence synchronization thread";
            _stopping = new ManualResetEvent(false);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _stopping.Set();
            _thread.Join();
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

        private void SynchronizeProc()
        {
            while (!_stopping.WaitOne(1000))
            {
                try
                {
                    _community.Synchronize();
                    LastError = null;
                }
                catch (Exception ex)
                {
                    LastError = ex.Message;
                }
            }
        }
    }
}
