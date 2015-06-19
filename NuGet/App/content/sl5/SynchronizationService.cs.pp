using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Correspondence;
using Correspondence.IsolatedStorage;
using Correspondence.BinaryHTTPClient;
using Correspondence.Memory;

namespace $rootnamespace$
{
    public class SynchronizationService
    {
        private const string ThisIndividual = "$rootnamespace$.Individual.this";

        private Community _community;
        private Individual _individual;

        public void Initialize()
        {
            var storage = IsolatedStorageStorageStrategy.Load();
            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);

            _community = new Community(storage);
            _community.AddAsynchronousCommunicationStrategy(communication);
            _community.Register<CorrespondenceModel>();
            _community.Subscribe(() => _individual);

            CreateIndividual();
			
            // Synchronize whenever the user has something to send.
            _community.FactAdded += delegate
            {
                _community.BeginSending();
            };

            // Periodically resume if there is an error.
            DispatcherTimer synchronizeTimer = new DispatcherTimer();
            synchronizeTimer.Tick += delegate
            {
                _community.BeginSending();
                _community.BeginReceiving();
            };
            synchronizeTimer.Interval = TimeSpan.FromSeconds(60.0);
            synchronizeTimer.Start();

            // And synchronize on startup.
            _community.BeginSending();
            _community.BeginReceiving();
        }

        public void InitializeDesignMode()
        {
            _community = new Community(new MemoryStorageStrategy());
            _community.Register<CorrespondenceModel>();

            CreateIndidualDesignData();
        }

        public Community Community
        {
            get { return _community; }
        }

        public Individual Individual
        {
            get { return _individual; }
        }

        private void CreateIndividual()
        {
            _individual = _community.LoadFact<Individual>(ThisIndividual);
            if (_individual == null)
            {
                _individual = _community.AddFact(new Individual());
                _community.SetFact(ThisIndividual, _individual);
            }
        }

        private void CreateIndidualDesignData()
        {
            var individual = _community.AddFact(new Individual());
            _individual = individual;
        }
    }
}
