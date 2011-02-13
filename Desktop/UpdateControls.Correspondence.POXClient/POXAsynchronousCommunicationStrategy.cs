using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;

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

		public void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Guid clientGuid, Action<FactTreeMemento, TimestampID> callback)
		{
            GetRequest request = new GetRequest
            {
                PivotTree = Translate.MementoToFactTree(pivotTree),
                PivotId = pivotId.key,
                Timestamp = timestamp.Key,
                ClientGuid = clientGuid.ToString()
            };
            POXHttpRequest.Begin<GetRequest, GetResponse>(
                _configuration.Endpoint,
                request,
                response => callback(
                    Translate.FactTreeToMemento(response.FactTree),
                    new TimestampID(response.FactTree.DatabaseId, response.Timestamp)),
                ex => callback(
                    new FactTreeMemento(pivotTree.DatabaseId),
                    new TimestampID(pivotTree.DatabaseId, timestamp.Key)));
        }

        public void BeginPost(FactTreeMemento messageBody, Guid clientGuid, Action<bool> callback)
        {
            PostRequest request = new PostRequest
            {
                MessageBody = Translate.MementoToFactTree(messageBody),
                ClientGuid = clientGuid.ToString()
            };
            POXHttpRequest.Begin<PostRequest, PostResponse>(
                _configuration.Endpoint,
                request,
                response => callback(true),
                ex => callback(false));
        }

		public event Action<FactTreeMemento> MessageReceived;
    }
}
