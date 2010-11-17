using System.Windows.Threading;
using System.Windows;

namespace UpdateControls.Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private static Dispatcher GetDispatcher()
        {
            return Deployment.Current.Dispatcher;
        }
    }
}