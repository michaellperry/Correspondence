using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.Memory;
using UpdateControls.Correspondence.SSCE;
using UpdateControls.Fields;

namespace $rootnamespace$
{
    public class SynchronizationService
    {
        private const string ThisIndividual = "$rootnamespace$.Individual.this";
        private static readonly Regex Punctuation = new Regex(@"[{}\-]");

        private Community _community;
        private Independent<Individual> _individual = new Independent<Individual>(
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
			_community.SetDesignMode();

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
					string randomId = Punctuation.Replace(Guid.NewGuid().ToString(), String.Empty).ToLower();
					individual = await _community.AddFactAsync(new Individual(randomId));
					await _community.SetFactAsync(ThisIndividual, individual);
				}
				Individual = individual;
			});
        }

        private void CreateIndividualDesignData()
        {
			_community.Perform(async delegate
			{
				var individual = await _community.AddFactAsync(new Individual("design"));
				Individual = individual;
			});
        }
    }
}
