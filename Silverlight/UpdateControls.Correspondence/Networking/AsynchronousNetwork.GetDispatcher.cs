using System;
using System.Windows;

namespace UpdateControls.Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private static void RunOnUIThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}