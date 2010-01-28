using System;
using System.Windows;
using GameModel;
using Reversi.Client.Synchronization;
using Reversi.Client.View;
using Reversi.Client.ViewModel;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.WebServiceClient;
using UpdateControls.XAML;

namespace Reversi.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Person _person;
        private SynchronizationThread _synchronizationThread;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Community community = new Community(new MemoryStorageStrategy())
                .AddCommunicationStrategy(new WebServiceCommunicationStrategy())
                .RegisterAssembly(typeof(GameQueue))
                .AddInterest(() => _person.OutstandingGameRequests)
                .AddInterest(() => _person.UnfinishedGames);

            _synchronizationThread = new SynchronizationThread(community);

            GameQueue gameQueue = community.AddFact(new GameQueue("http://correspondence.cloudapp.net/reversi/1"));
            _person = community.AddFact(new Person());
            GameViewModel gameViewModel = new GameViewModel(
                _person,
                gameQueue,
                _synchronizationThread);

            MainWindow = new MainWindow();
            MainWindow.DataContext = ForView.Wrap(gameViewModel);
            MainWindow.Closed += new EventHandler(MainWindow_Closed);
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
