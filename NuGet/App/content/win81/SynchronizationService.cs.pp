using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Correspondence;
using Correspondence.BinaryHTTPClient;
using Correspondence.FileStream;
using Correspondence.Memory;
using Assisticant.Fields;
using Windows.Storage;
using Windows.UI.Xaml;

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
            var storage = new FileStreamStorageStrategy();
            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);

            _community = new Community(storage);
            _community.AddAsynchronousCommunicationStrategy(communication);
            _community.Register<CorrespondenceModel>();
            _community.Subscribe(() => Individual);

            // Synchronize periodically.
            DispatcherTimer timer = new DispatcherTimer();
            int timeoutSeconds = Math.Min(http.Configuration.TimeoutSeconds, 30);
            timer.Interval = TimeSpan.FromSeconds(5 * timeoutSeconds);
            timer.Tick += delegate(object sender, object e)
            {
                Synchronize();
            };
            timer.Start();

            CreateIndividual(http);

            // Synchronize whenever the user has something to send.
            _community.FactAdded += delegate
            {
                Synchronize();
            };

            // Synchronize when the network becomes available.
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += (sender, e) =>
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                    Synchronize();
            };

            // And synchronize on startup or resume.
            Synchronize();
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

        public void Synchronize()
        {
            _community.BeginSending();
            _community.BeginReceiving();
        }

        private void CreateIndividual(HTTPConfigurationProvider http)
        {
			_community.Perform(async delegate
			{
				Individual individual = await _community.LoadFactAsync<Individual>(ThisIndividual);
				if (individual == null)
				{
					individual = await _community.AddFactAsync(new Individual());
					await _community.SetFactAsync(ThisIndividual, individual);
				}
				Individual = individual;
				http.Individual = individual;
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
