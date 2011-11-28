using System;
using System.Windows.Threading;

namespace UpdateControls.Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private void RunOnUIThread(Action action)
        {
            if (_model.ClientApp)
                Dispatcher.CurrentDispatcher.BeginInvoke(action);
            else
                System.Threading.ThreadPool.QueueUserWorkItem(o => action());
        }
    }
}