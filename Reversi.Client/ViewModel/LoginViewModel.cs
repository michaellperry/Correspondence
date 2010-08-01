using System.Linq;
using System.Windows.Input;
using Reversi.Model;
using UpdateControls.XAML;

namespace Reversi.Client.ViewModel
{
    public class LogonViewModel
    {
        private Machine _machine;
        private LoginNavigationModel _navigation = new LoginNavigationModel();

        public LogonViewModel(Machine machine)
        {
            _machine = machine;
        }

        public string Username
        {
            get { return _navigation.UserName; }
            set { _navigation.UserName = value; }
        }

        public ICommand LogOn
        {
            get
            {
                return MakeCommand
                    .When(() => !string.IsNullOrEmpty(_navigation.UserName))
                    .Do(() => _machine.LogOnUser(_navigation.UserName));
            }
        }

        public ICommand LogOff
        {
            get
            {
                return MakeCommand
                    .Do(() => _machine.LogOffUser());
            }
        }

        public bool IsLoggedOn
        {
            get
            {
                return _machine.ActiveLogOns.Any();
            }
        }
    }
}
