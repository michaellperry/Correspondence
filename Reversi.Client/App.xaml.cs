using System;
using System.Windows;
using Reversi.Model;
using Reversi.Client.Synchronization;
using Reversi.Client.View;
using Reversi.Client.ViewModel;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.WebServiceClient;
using UpdateControls.XAML;

namespace Reversi.Client
{
    public partial class App : Application
    {
        private const string ThisMachineName = "Reversi.Model.Machine.thisMachine";

        private SynchronizationThread _synchronizationThread;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Community community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(new WebServiceCommunicationStrategy())
                .RegisterAssembly(typeof(Machine));

            // Load or create the Machine fact.
            Machine thisMachine = community.LoadFact<Machine>(ThisMachineName);
            if (thisMachine == null)
            {
                thisMachine = community.AddFact(new Machine());
                community.SetFact(ThisMachineName, thisMachine);
            }

            _synchronizationThread = new SynchronizationThread(community);

            MainViewModel mainViewModel = new MainViewModel(
                thisMachine,
                _synchronizationThread);

            MainWindow = new MainWindow();
            MainWindow.DataContext = ForView.Wrap(mainViewModel);
            MainWindow.Closed += MainWindow_Closed;
            MainWindow.Show();

            community.FactAdded += () => _synchronizationThread.Wake();
            _synchronizationThread.Start();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _synchronizationThread.Stop();
        }
    }
}
