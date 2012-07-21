using System;
using System.Linq;

namespace $rootnamespace$.ViewModels
{
    public class MainViewModel
    {
        private Individual _individual;
        private SynchronizationService _synhronizationService;

        public MainViewModel(Individual individual, SynchronizationService synhronizationService)
        {
            _individual = individual;
            _synhronizationService = synhronizationService;
        }

        public bool Synchronizing
        {
            get { return _synhronizationService.Synchronizing; }
        }
    }
}
