using System.Linq;
using Reversi.Client.Synchronization;
using Reversi.Model;

namespace Reversi.Client.ViewModel
{
    public class MainViewModel
    {
        private Machine _machine;
        private SynchronizationThread _synchronizationThread;
        private MainNavigationModel _mainNavigation = new MainNavigationModel();

        public MainViewModel(Machine machine, SynchronizationThread synchronizationThread)
        {
            _machine = machine;
            _synchronizationThread = synchronizationThread;
        }

        public LogonViewModel Logon
        {
            get { return new LogonViewModel(_machine); }
        }

        public string LastError
        {
            get { return _synchronizationThread.LastError; }
        }

        public GameListViewModel GameList
        {
            get
            {
                LogOn activeLogOn = _machine.ActiveLogOns.LastOrDefault();
                if (activeLogOn == null)
                    return null;
                return new GameListViewModel(activeLogOn.User, _mainNavigation);
            }
        }

        public GameViewModel SelectedGame
        {
            get
            {
                return _mainNavigation.SelectedPlayer == null
                    ? null
                    : new GameViewModel(_mainNavigation.SelectedPlayer);
            }
        }
    }
}
