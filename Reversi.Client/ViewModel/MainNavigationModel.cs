using Reversi.Model;
using UpdateControls;

namespace Reversi.Client.ViewModel
{
    public class MainNavigationModel
    {
        private Player _selectedPlayer;
        private Independent _indSelectedPlayer = new Independent();

        public Player SelectedPlayer
        {
            get
            {
                _indSelectedPlayer.OnGet();
                return _selectedPlayer;
            }
            set
            {
                if (value != null)
                {
                    _indSelectedPlayer.OnSet();
                    _selectedPlayer = value;
                }
            }
        }
    }
}
