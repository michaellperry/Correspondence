using System;
using System.Linq;
using System.Windows;
using Reversi.Client.Synchronization;
using Reversi.Client.View;
using Reversi.Client.ViewModel;
using Reversi.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.POXClient;
using UpdateControls.XAML;

namespace Reversi.Client
{
    public partial class App : Application
    {
        private const string ThisMachineName = "Reversi.Model.Machine.thisMachine";

        private Machine _thisMachine;
        private SynchronizationThread _synchronizationThread;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Community community = new Community(new MemoryStorageStrategy())
                .AddAsynchronousCommunicationStrategy(new POXAsynchronousCommunicationStrategy(new POXClientConfigurationProvider()))
                .RegisterAssembly(typeof(Machine))
                .Subscribe(() => _thisMachine.ActiveLogOns
                    .Select(l => l.User)
                )
                .Subscribe(() => _thisMachine.ActiveLogOns
                    .SelectMany(l => l.User.ActivePlayers)
                    .Select(p => p.Game)
                );

            // Load or create the Machine fact.
            _thisMachine = community.LoadFact<Machine>(ThisMachineName);
            if (_thisMachine == null)
            {
                _thisMachine = community.AddFact(new Machine());
                community.SetFact(ThisMachineName, _thisMachine);
            }

            _synchronizationThread = new SynchronizationThread(community);

            MainViewModel mainViewModel = new MainViewModel(
                _thisMachine,
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
