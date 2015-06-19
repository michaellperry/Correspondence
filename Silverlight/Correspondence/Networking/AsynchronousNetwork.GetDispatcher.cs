using System;
using System.Windows;

namespace Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private static void RunOnUIThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(action);
        }
    }
}