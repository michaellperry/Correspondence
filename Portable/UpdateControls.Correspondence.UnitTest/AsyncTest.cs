﻿using System;
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
        private Community _community;

        protected void InitializeAsyncTest()
        {
            _memory = new AsyncMemoryStorageStrategy();
        }

        protected Community Community
        {
            get { return _community; }
            set { _community = value; }
        }

        protected async Task QuiesceAllAsync()
        {
            while (await _community.QuiesceAsync()) ;
        }
    }
}
