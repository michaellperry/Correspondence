﻿using System;
using System.Collections.Generic;
using System.Linq;
using Correspondence.Mementos;
using Correspondence.Strategy;

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

        public void BeginGetMany(FactTreeMemento pivotTree, List<PivotMemento> pivots, Guid clientGuid, Action<FactTreeMemento, IEnumerable<PivotMemento>> callback, Action<Exception> error)
        {
            if (_configuration.APIKey == DefaultAPIKey)
            {
                error(new ArgumentException("Please register for an API key at http://CorrespondenceCloud.com"));
                return;
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

        public void BeginPost(FactTreeMemento messageBody, Guid clientGuid, List<UnpublishMemento> unpublishedMessages, Action<bool> callback, Action<Exception> error)
        {
            if (_configuration.APIKey == DefaultAPIKey)
            {
                callback(false);
                error(new ArgumentException("Please register for an API key at http://qedcode.com/correspondence"));
                return;
            }

            PostRequest request = new PostRequest
            {
                Domain = _configuration.APIKey,
                MessageBody = messageBody,
                ClientGuid = clientGuid.ToString(),
                UnpublishedMessages = unpublishedMessages
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

        public void Interrupt(Guid clientGuid)
        {
            if (_configuration.APIKey == DefaultAPIKey)
                return;

            InterruptRequest request = new InterruptRequest
            {
                Domain = _configuration.APIKey,
                ClientGuid = clientGuid.ToString()
            };
            BinaryHTTPRequest.Begin(
                _configuration,
                request,
                response => { },
                ex => { });
        }

        public void Notify(FactTreeMemento messageBody, FactID pivotId, Guid clientGuid, string text1, string text2)
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
            BinaryHTTPRequest.Begin(
                _configuration,
                request,
                response => { },
                ex => { });
        }

        public event Action<FactTreeMemento> MessageReceived;
    }
}
