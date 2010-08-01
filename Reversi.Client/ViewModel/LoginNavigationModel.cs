using UpdateControls;

namespace Reversi.Client.ViewModel
{
    public class LoginNavigationModel
    {
        private string _userName;
        private Independent _indUserName = new Independent();

        public string UserName
        {
            get
            {
                _indUserName.OnGet();
                return _userName;
            }
            set
            {
                _indUserName.OnSet();
                _userName = value;
            }
        }
    }
}
