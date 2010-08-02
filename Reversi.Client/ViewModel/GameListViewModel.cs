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
        private GameListNavigationModel _navigation = new GameListNavigationModel();

        public GameListViewModel(User user)
        {
            _user = user;
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
                    .Do(() => _user.Challenge(_navigation.OpponentName));
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

        //public IEnumerable<GameSummaryViewModel> Wins { get; private set; }
        //public IEnumerable<GameSummaryViewModel> Losses { get; private set; }

        public GameSummaryViewModel SelectedGame { get; set; }

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