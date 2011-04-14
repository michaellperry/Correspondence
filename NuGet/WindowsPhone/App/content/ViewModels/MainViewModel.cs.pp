using System;
using System.Linq;
using $rootnamespace$.Models;

namespace $rootnamespace$.ViewModels
{
    public class MainViewModel
    {
        private Identity _identity;

        public MainViewModel(Identity identity)
        {
            _identity = identity;
        }
    }
}
