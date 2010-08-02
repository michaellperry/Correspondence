using UpdateControls;

namespace Reversi.Client.ViewModel
{
    public class GameListNavigationModel
    {
        private string _opponentName;
        private Independent _indOpponentName = new Independent();

        public string OpponentName
        {
            get
            {
                _indOpponentName.OnGet();
                return _opponentName;
            }
            set
            {
                _indOpponentName.OnSet();
                _opponentName = value;
            }
        }
    }
}
