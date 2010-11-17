using System.Windows.Threading;

namespace UpdateControls.Correspondence.Networking
{
    partial class AsynchronousNetwork
    {
        private static Dispatcher GetDispatcher()
        {
            return Dispatcher.CurrentDispatcher;
        }
    }
}