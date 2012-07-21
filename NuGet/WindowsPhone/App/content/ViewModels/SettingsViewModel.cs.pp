using System;
using System.Linq;

namespace $rootnamespace$.ViewModels
{
    public class SettingsViewModel
    {
        private Individual _individual;

        public SettingsViewModel(Individual individual)
        {
            _individual = individual;
        }

        public bool EnableToastNotification
        {
            get { return _individual.ToastNotificationEnabled; }
            set { _individual.ToastNotificationEnabled = value; }
        }
    }
}
