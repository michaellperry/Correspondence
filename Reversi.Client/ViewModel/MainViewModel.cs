using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reversi.Client.Synchronization;
using Reversi.Model;

namespace Reversi.Client.ViewModel
{
    public class MainViewModel
    {
        private Machine _machine;
        private SynchronizationThread _synchronizationThread;

        public MainViewModel(Machine machine, SynchronizationThread synchronizationThread)
        {
            _machine = machine;
            _synchronizationThread = synchronizationThread;
        }

        public LogonViewModel Logon
        {
            get
            {
                return new LogonViewModel(_machine);
            }
        }
    }
}
