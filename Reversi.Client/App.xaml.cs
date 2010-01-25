using System.Windows;
using GameModel;
using Reversi.Client.View;
using Reversi.Client.ViewModel;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.Memory;
using UpdateControls.XAML;
using GameService;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Threading;
using UpdateControls;
using System;

namespace Reversi.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private GameQueueService _service;
        private Dependent _depService;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Community community = new Community(new MemoryStorageStrategy())
                .RegisterAssembly(typeof(GameQueue));

            GameQueue gameQueue = community.AddFact(new GameQueue("http://correspondence.azure.com/reversi/1"));
            GameViewModel gameViewModel = new GameViewModel(
                community.AddFact(new Person()),
                gameQueue);

            MainWindow = new MainWindow();
            MainWindow.DataContext = ForView.Wrap(gameViewModel);
            MainWindow.Show();

            Window otherWindow = new MainWindow();
            otherWindow.DataContext = ForView.Wrap(new GameViewModel(community.AddFact(new Person()), gameQueue));
            otherWindow.Show();

            _service = new GameQueueService(gameQueue);
            _depService = new Dependent(RunService);
            _depService.Invalidated += () => Dispatcher.BeginInvoke(new Action(() => _depService.OnGet()));
            _depService.OnGet();
        }


        private void RunService()
        {
            List<GameRequest> queue = _service.Queue.ToList();
            queue.Reverse();
            _service.Process(queue);
        }
    }
}
