using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UpdateControls.Correspondence.Memory;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.UnitTest
{
    public class AsyncTest
    {
        protected AsyncMemoryStorageStrategy _memory;
        private bool _done = false;
        private Thread _background;
        private Community _community;

        protected void InitializeAsyncTest()
        {
            _memory = new AsyncMemoryStorageStrategy();
        }

        protected void QuiescePeriodically()
        {
            _background = new Thread(delegate(object o)
            {
                while (!_done)
                {
                    Thread.Sleep(10);
                    _memory.Quiesce();
                }
            });
            _background.Start();
        }

        protected void Done()
        {
            _done = true;
            _background.Join();
        }

        protected Community Community
        {
            get { return _community; }
            set { _community = value; }
        }

        protected async Task QuiesceAllAsync()
        {
            while (_memory.Quiesce() || await _community.QuiesceAsync()) ;
        }
    }
}
