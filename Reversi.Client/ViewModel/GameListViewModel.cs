using System.Collections.Generic;
using System.Windows.Input;
using UpdateControls.XAML;

namespace Reversi.Client
{
    public class GameListViewModel
	{
		public string OpponentName { get; set; }
        public ICommand Challenge
        {
            get
            {
                return MakeCommand
                    .Do(() => { });
            }
        }

        public IEnumerable<GameSummaryViewModel> YourMove { get; private set; }
        public IEnumerable<GameSummaryViewModel> TheirMove { get; private set; }
        public IEnumerable<GameSummaryViewModel> Wins { get; private set; }
        public IEnumerable<GameSummaryViewModel> Losses { get; private set; }

        public GameSummaryViewModel SelectedGame { get; set; }
    }
}