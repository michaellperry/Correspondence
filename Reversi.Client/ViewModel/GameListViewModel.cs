using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Reversi.Model;
using UpdateControls.XAML;

namespace Reversi.Client.ViewModel
{
    public class GameListViewModel
	{
        private User _user;
        private MainNavigationModel _mainNavigation;
        private GameListNavigationModel _navigation = new GameListNavigationModel();

        public GameListViewModel(User user, MainNavigationModel mainNavigation)
        {
            _user = user;
            _mainNavigation = mainNavigation;
        }

        public string OpponentName
        {
            get { return _navigation.OpponentName; }
            set { _navigation.OpponentName = value; }
        }

        public ICommand Challenge
        {
            get
            {
                return MakeCommand
                    .When(() => !string.IsNullOrEmpty(_navigation.OpponentName))
                    .Do(() =>
                    {
                        _mainNavigation.SelectedPlayer = _user.Challenge(_navigation.OpponentName);
                        _navigation.OpponentName = null;
                    });
            }
        }

        public IEnumerable<GameSummaryViewModel> YourMove
        {
            get
            {
                return _user.ActivePlayers
                    .Select(p => new GameSummaryViewModel(p))
                    .Where(vm => vm.MyTurn);
            }
        }

        public IEnumerable<GameSummaryViewModel> TheirMove
        {
            get
            {
                return _user.ActivePlayers
                    .Select(p => new GameSummaryViewModel(p))
                    .Where(vm => !vm.MyTurn);
            }
        }

        public IEnumerable<GameSummaryViewModel> Wins
        {
            get
            {
                return _user.FinishedPlayers
                    .Where(p => p.Game.Outcome.Winner == p)
                    .Select(p => new GameSummaryViewModel(p));
            }
        }

        public IEnumerable<GameSummaryViewModel> Losses
        {
            get
            {
                return _user.FinishedPlayers
                    .Where(p => p.Game.Outcome.Winner != p)
                    .Select(p => new GameSummaryViewModel(p));
            }
        }

        public GameSummaryViewModel SelectedGame
        {
            get
            {
                return _mainNavigation.SelectedPlayer == null
                    ? null
                    : new GameSummaryViewModel(_mainNavigation.SelectedPlayer);
            }
            set
            {
            	_mainNavigation.SelectedPlayer = value == null
                    ? null
                    : value.Player;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            GameListViewModel that = obj as GameListViewModel;
            if (that == null)
                return false;
            return this._user == that._user;
        }

        public override int GetHashCode()
        {
            return _user.GetHashCode();
        }
    }
}