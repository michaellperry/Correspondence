using System;
using System.Collections.Generic;
using System.Linq;
using Correspondence.Mementos;
using Correspondence.Strategy;
using System.Threading.Tasks;

namespace Correspondence.BinaryHTTPClient
{
    public partial class BinaryHTTPAsynchronousCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
        private const string DefaultAPIKey = "<<Your API key>>";

        private HTTPConfiguration _configuration;
        private IHTTPConfigurationProvider _configurationProvider;

        public BinaryHTTPAsynchronousCommunicationStrategy(IHTTPConfigurationProvider configurationProvider)
		{
            _configurationProvider = configurationProvider;
			_configuration = configurationProvider.Configuration;

            Initialize();
		}

        partial void Initialize();

        public string ProtocolName
        {
            get { return "http://correspondence.net/bin"; }
        }

        public string PeerName
        {
            get { return _configuration.Endpoint; }
        }

        public async Task<GetManyResultMemento> GetManyAsync(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid)
        {
            if (_configuration.APIKey == DefaultAPIKey)
            {
                throw new ArgumentException("Please register for an API key at http://correspondencecloud.com");
            }

            GetManyRequest request = new GetManyRequest
            {
                Domain = _configuration.APIKey,
                PivotTree = pivotTree,
                PivotIds = pivots
                    .Select(pivot => new FactTimestamp { FactId = pivot.PivotId.key, TimestampId = pivot.Timestamp.Key })
                    .ToList(),
                ClientGuid = clientGuid.ToString(),
                TimeoutSeconds = _configuration.TimeoutSeconds
            };
            GetManyResponse response = await BinaryHTTPRequest.SendAsync(
                _configuration,
                request) as GetManyResponse;
            if (response == null)
                throw new InvalidOperationException("The server returned an incorrect response.");
            GetManyResultMemento result = new GetManyResultMemento(
                response.FactTree,
                response.PivotIds
                    .Select(pivot => new PivotMemento(
                        new FactID { key = pivot.FactId },
                        new TimestampID(0L, pivot.TimestampId)))
                    .ToList()
            );
            return result;
        }

        public async Task PostAsync(FactTreeMemento messageBody, Guid clientGuid, List<UnpublishMemento> unpublishedMessages)
        {
            if (_configuration.APIKey == DefaultAPIKey)
            {
                throw new ArgumentException("Please register for an API key at http://correspondencecloud.com");
            }

            PostRequest request = new PostRequest
            {
                Domain = _configuration.APIKey,
                MessageBody = messageBody,
                ClientGuid = clientGuid.ToString(),
                UnpublishedMessages = unpublishedMessages
            };
            PostResponse response = await BinaryHTTPRequest.SendAsync(
                _configuration,
                request) as PostResponse;
            if (response == null)
                throw new InvalidOperationException("The server returned an incorrect response.");
        }

        public async Task InterruptAsync(Guid clientGuid)
        {
            if (_configuration.APIKey == DefaultAPIKey)
                return;

            InterruptRequest request = new InterruptRequest
            {
                Domain = _configuration.APIKey,
                ClientGuid = clientGuid.ToString()
            };
            InterruptResponse response = await BinaryHTTPRequest.SendAsync(
                _configuration,
                request) as InterruptResponse;
            if (response == null)
                throw new InvalidOperationException("The server returned an incorrect response.");
        }

        public async Task NotifyAsync(FactTreeMemento messageBody, FactID pivotId, Guid clientGuid, string text1, string text2)
        {
            if (_configuration.APIKey == DefaultAPIKey)
                return;

            NotifyRequest request = new NotifyRequest
            {
                Domain = _configuration.APIKey,
                PivotTree = messageBody,
                PivotId = pivotId.key,
                ClientGuid = clientGuid.ToString(),
                Text1 = text1,
                Text2 = text2
            };
            NotifyResponse response = await BinaryHTTPRequest.SendAsync(
                _configuration,
                request) as NotifyResponse;
            if (response == null)
                throw new InvalidOperationException("The server returned an incorrect response.");
        }

        public event Action<FactTreeMemento> MessageReceived;
    }
}
