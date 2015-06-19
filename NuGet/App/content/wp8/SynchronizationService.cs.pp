using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Correspondence;
using Correspondence.FileStream;
using Correspondence.BinaryHTTPClient;
using Correspondence.BinaryHTTPClient.Notification;
using Assisticant.Fields;
using Correspondence.Memory;

namespace $rootnamespace$
{
    public class SynchronizationService
    {
        private Community _community;
        private Observable<Individual> _individual = new Observable<Individual>(
            Individual.GetNullInstance());

        public void Initialize()
        {
            var storage = new FileStreamStorageStrategy();
            var http = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(http);
            var notification = new WindowsPhoneNotificationStrategy(http);
            communication.SetNotificationStrategy(notification);

            _community = new Community(storage);
            _community.AddAsynchronousCommunicationStrategy(communication);
            _community.Register<CorrespondenceModel>();
            _community.Subscribe(() => Individual);

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

        public void InitializeDesignData()
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

        private void CreateIndividual(HTTPConfigurationProvider http)
        {
			_community.Perform(async delegate
			{
				var individual = await _community.AddFactAsync(new Individual());
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

        public void Synchronize()
        {
            _community.BeginSending();
            _community.BeginReceiving();
        }
    }
}
