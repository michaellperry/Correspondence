using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.IsolatedStorage;
using UpdateControls.Correspondence.POXClient;
using $rootnamespace$.Models;

namespace $rootnamespace$
{
    public class SynchronizationService
    {
        private NavigationModel _navigationModel;
        private Community _community;

        public SynchronizationService(NavigationModel navigationModel)
        {
            _navigationModel = navigationModel;
        }

        public void Initialize()
        {
            POXConfigurationProvider configurationProvider = new POXConfigurationProvider();
            _community = new Community(IsolatedStorageStorageStrategy.Load())
                .AddAsynchronousCommunicationStrategy(new POXAsynchronousCommunicationStrategy(configurationProvider))
                .Register<CorrespondenceModel>()
                .Subscribe(() => _navigationModel.CurrentUser)
                ;

            // Synchronize whenever the user has something to send.
            _community.FactAdded += delegate
            {
                Synchronize();
            };

            // And synchronize on startup.
            Synchronize();
        }

        public Community Community
        {
            get { return _community; }
        }

        public void Synchronize()
        {
            _community.BeginSynchronize(delegate(IAsyncResult result)
            {
                if (_community.EndSynchronize(result))
                    Synchronize();
            }, null);
        }
    }
}
