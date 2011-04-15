using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using $rootnamespace$.Models;
using UpdateControls.Correspondence;
using UpdateControls.XAML;

namespace $rootnamespace$.ViewModels
{
    public class MainViewModel
    {
        private Community _community;
        private NavigationModel _navigationModel;

        public MainViewModel(Community community, NavigationModel navigationModel)
        {
            _community = community;
            _navigationModel = navigationModel;
        }
    }
}
