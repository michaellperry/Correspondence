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
        private Community _community;
        private Identity _identity;

        public void Initialize()
        {
            POXConfigurationProvider configurationProvider = new POXConfigurationProvider();
            _community = new Community(IsolatedStorageStorageStrategy.Load())
                .AddAsynchronousCommunicationStrategy(new POXAsynchronousCommunicationStrategy(configurationProvider))
                .Register<CorrespondenceModel>()
                .Subscribe(() => _identity)
                ;

            _identity = _community.AddFact(new Identity("Example"));
            configurationProvider.Identity = _identity;

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

        public Identity Identity
        {
            get { return _identity; }
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
