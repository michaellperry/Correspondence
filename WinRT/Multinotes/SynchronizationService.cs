using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using Multinotes.Model;
using UpdateControls.Correspondence.Memory;
using System.Net.NetworkInformation;
using UpdateControls.Fields;
using UpdateControls.Correspondence.FileStream;
using System.IO;
using UpdateControls.XAML.Wrapper;

namespace Multinotes
{
    public class SynchronizationService
    {
        private const string ThisIndividual = "Multinotes.Individual.1.this";

        private Community _community;
        private Independent<Individual> _individual = new Independent<Individual>();

        public async void Initialize()
        {
            HTTPConfigurationProvider configurationProvider = new HTTPConfigurationProvider();
            _community = new Community(new FileStreamStorageStrategy())
                .AddAsynchronousCommunicationStrategy(new BinaryHTTPAsynchronousCommunicationStrategy(configurationProvider))
                .Register<CorrespondenceModel>()
                .Subscribe(() => Individual)
                .Subscribe(() => Individual == null
                    ? null
                    : Individual.MessageBoards)
                ;

            Individual individual = await _community.LoadFactAsync<Individual>(ThisIndividual);
            if (individual == null)
            {
                individual = await _community.AddFactAsync(new Individual(Guid.NewGuid().ToString()));
                await _community.SetFactAsync(ThisIndividual, individual);
                await individual.JoinMessageBoardAsync("Video Games");
                await individual.JoinMessageBoardAsync("Tulsa TechFest 2012");
            }
            _individual.Value = individual;
            configurationProvider.Individual = individual;

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

        public Community Community
        {
            get { return _community; }
        }

        public Individual Individual
        {
            get { return _individual; }
        }

        public void Synchronize()
        {
            _community.BeginSending();
            _community.BeginReceiving();
        }

        public bool Synchronizing
        {
            get { return _community.Synchronizing; }
        }

        public Exception LastException
        {
            get { return _community.LastException; }
        }
    }
}
