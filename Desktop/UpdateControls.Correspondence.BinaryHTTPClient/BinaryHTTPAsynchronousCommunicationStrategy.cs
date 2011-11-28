using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.BinaryHTTPClient
{
    public partial class BinaryHTTPAsynchronousCommunicationStrategy : IAsynchronousCommunicationStrategy
    {
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
            get { return "http://correspondence.updatecontrols.net/bin"; }
        }

        public string PeerName
        {
            get { return _configuration.Endpoint; }
        }

        public void BeginGetMany(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid, Action<FactTreeMemento, IEnumerable<PivotMemento>> callback, Action<Exception> error)
        {
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
            BinaryHTTPRequest.Begin(
                _configuration,
                request,
                response => callback(
                    ((GetManyResponse)response).FactTree,
                    ((GetManyResponse)response).PivotIds
                        .Select(pivot => new PivotMemento(
                            new FactID { key = pivot.FactId },
                            new TimestampID(0L, pivot.TimestampId)))),
                ex =>
                {
                    error(ex);
                });
        }

        public void BeginPost(FactTreeMemento messageBody, Guid clientGuid, Action<bool> callback, Action<Exception> error)
        {
            PostRequest request = new PostRequest
            {
                Domain = _configuration.APIKey,
                MessageBody = messageBody,
                ClientGuid = clientGuid.ToString()
            };
            BinaryHTTPRequest.Begin(
                _configuration,
                request,
                response => callback(true),
                ex =>
                {
                    callback(false);
                    error(ex);
                });
        }

        public event Action<FactTreeMemento> MessageReceived;
    }
}
