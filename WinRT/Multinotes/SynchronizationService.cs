using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Multinotes.Model;
using UpdateControls.Correspondence;
using UpdateControls.Correspondence.BinaryHTTPClient;
using UpdateControls.Correspondence.BinaryHTTPClient.Notification;
using UpdateControls.Correspondence.FileStream;
using UpdateControls.Fields;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Multinotes
{
    public class SynchronizationService
    {
        private const string ThisIndividual = "Multinotes.Individual.1.this";
        private static readonly Regex Punctuation = new Regex(@"[{}\-]");

        private Community _community;
        private Independent<Individual> _individual = new Independent<Individual>();

        public async void Initialize()
        {
            /////////////////////////////////////////
            // Local storage
            var storage = new FileStreamStorageStrategy();
            _community = new Community(storage);

            /////////////////////////////////////////
            // Communication
            var configurationProvider = new HTTPConfigurationProvider();
            var communication = new BinaryHTTPAsynchronousCommunicationStrategy(configurationProvider);
            //var notification = new WindowsNotificationStrategy(configurationProvider);
            //communication.SetNotificationStrategy(notification);
            _community.AddAsynchronousCommunicationStrategy(communication);

            /////////////////////////////////////////
            // Model
            _community.Register<CorrespondenceModel>();

            /////////////////////////////////////////
            // Subscription
            _community
                .Subscribe(() => Individual)
                .Subscribe(() => Individual == null
                    ? null
                    : Individual.MessageBoards)
                ;
            /////////////////////////////////////////

            // Synchronize periodically.
            DispatcherTimer timer = new DispatcherTimer();
            int timeoutSeconds = Math.Min(configurationProvider.Configuration.TimeoutSeconds, 30);
            timer.Interval = TimeSpan.FromSeconds(5 * timeoutSeconds);
            timer.Tick += delegate(object sender, object e)
            {
                Synchronize();
            };
            timer.Start();

            Individual individual = await _community.LoadFactAsync<Individual>(ThisIndividual);
            if (individual == null)
            {
                individual = await _community.AddFactAsync(new Individual(Guid.NewGuid().ToString()));
                await _community.SetFactAsync(ThisIndividual, individual);
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

        private async Task LogExceptionAsync(Exception x)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "debug.log", CreationCollisionOption.OpenIfExists);
            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                await Task.Run(delegate
                {
                    writer.WriteLine(x.ToString());
                });
            }
        }
    }
}
