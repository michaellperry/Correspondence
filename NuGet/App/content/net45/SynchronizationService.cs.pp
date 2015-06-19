using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using Correspondence;
using Correspondence.BinaryHTTPClient;
using Correspondence.Memory;
using Correspondence.SSCE;
using Assisticant.Fields;

namespace $rootnamespace$
{
    public class SynchronizationService
    {
        private const string ThisIndividual = "$rootnamespace$.Individual.this";

        private Community _community;
        private Observable<Individual> _individual = new Observable<Individual>(
            Individual.GetNullInstance());

        public void Initialize()
        {
            string correspondenceDatabase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CorrespondenceApp", "$rootnamespace$", "Correspondence.sdf");
            var storage = new SSCEStorageStrategy(correspondenceDatabase);
            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);

            _community = new Community(storage);
            _community.AddAsynchronousCommunicationStrategy(communication);
            _community.Register<CorrespondenceModel>();
            _community.Subscribe(() => Individual);

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

            CreateIndividualDesignData();
        }

        public Community Community
        {
            get { return _community; }
        }

        public Individual Individual
        {
            get
            {
                lock (this)
                {
                    return _individual;
                }
            }
            private set
            {
                lock (this)
                {
                    _individual.Value = value;
                }
            }
        }

        private void CreateIndividual()
        {
			_community.Perform(async delegate
			{
				var individual = await _community.LoadFactAsync<Individual>(ThisIndividual);
				if (individual == null)
				{
					individual = await _community.AddFactAsync(new Individual());
					await _community.SetFactAsync(ThisIndividual, individual);
				}
				Individual = individual;
			});
        }

        private void CreateIndividualDesignData()
        {
			_community.Perform(async delegate
			{
				var individual = await _community.AddFactAsync(new Individual());
				Individual = individual;
			});
        }
    }
}
