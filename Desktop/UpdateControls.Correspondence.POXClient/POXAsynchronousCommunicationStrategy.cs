using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Correspondence.POXClient
{
    public partial class POXAsynchronousCommunicationStrategy : IAsynchronousCommunicationStrategy
	{
        private POXConfiguration _configuration;
        private IPOXConfigurationProvider _configurationProvider;

		public POXAsynchronousCommunicationStrategy(IPOXConfigurationProvider configurationProvider)
		{
            _configurationProvider = configurationProvider;
			_configuration = configurationProvider.Configuration;

            Initialize();
		}

        partial void Initialize();

		public string ProtocolName
		{
			get { return "http://correspondence.updatecontrols.net/pox"; }
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
                PivotTree = Translate.MementoToFactTree(pivotTree),
                PivotIds = pivots
                    .Select(pivot => new ArrayOfPivotIdPivotId { FactId = pivot.PivotId.key, Timestamp = pivot.Timestamp.Key })
                    .ToArray(),
                ClientGuid = clientGuid.ToString(),
                TimeoutSeconds = _configuration.TimeoutSeconds
            };
            POXHttpRequest.Begin<GetManyRequest, GetManyResponse>(
                _configuration,
                request,
                response => callback(
                    Translate.FactTreeToMemento(response.FactTree),
                    response.PivotIds
                        .Select(pivot => new PivotMemento(
                            new FactID { key = pivot.FactId ?? 0L },
                            new TimestampID(0L, pivot.Timestamp)))),
                ex => callback(
                    new FactTreeMemento(pivotTree.DatabaseId),
                    Enumerable.Empty<PivotMemento>()));
        }

        public void BeginPost(FactTreeMemento messageBody, Guid clientGuid, Action<bool> callback, Action<Exception> error)
        {
            PostRequest request = new PostRequest
            {
                Domain = _configuration.APIKey,
                MessageBody = Translate.MementoToFactTree(messageBody),
                ClientGuid = clientGuid.ToString()
            };
            POXHttpRequest.Begin<PostRequest, PostResponse>(
                _configuration,
                request,
                response => callback(true),
                ex => callback(false));
        }

		public event Action<FactTreeMemento> MessageReceived;
    }
}
