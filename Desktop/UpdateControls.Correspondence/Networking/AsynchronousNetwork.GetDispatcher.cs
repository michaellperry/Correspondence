using System;
using System.Windows.Threading;

namespace UpdateControls.Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private static void RunOnUIThread(Action action)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(action);
        }
    }
}