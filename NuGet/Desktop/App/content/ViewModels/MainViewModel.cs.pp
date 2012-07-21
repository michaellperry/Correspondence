using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.Correspondence;
using UpdateControls.XAML;

namespace $rootnamespace$.ViewModels
{
    public class MainViewModel
    {
        private Community _community;
        private SynchronizationService _synhronizationService;

        public MainViewModel(Community community, SynchronizationService synhronizationService)
        {
            _community = community;
            _synhronizationService = synhronizationService;
        }

        public bool Synchronizing
        {
            get { return _synhronizationService.Synchronizing; }
        }
    }
}
