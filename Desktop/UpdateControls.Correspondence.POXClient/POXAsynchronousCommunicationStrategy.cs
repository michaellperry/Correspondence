using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.POXClient.Contract;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.POXClient
{
    public class POXAsynchronousCommunicationStrategy : IAsynchronousCommunicationStrategy
	{
		private static XmlSerializer GetRequestSerializer = new XmlSerializer(typeof(GetRequest));
		private static XmlSerializer GetResponseSerializer = new XmlSerializer(typeof(GetResponse));
		private static XmlSerializer PostRequestSerializer = new XmlSerializer(typeof(PostRequest));
		private static XmlSerializer PostResponseSerializer = new XmlSerializer(typeof(PostResponse));

        private POXConfiguration _configuration;

		public POXAsynchronousCommunicationStrategy(IPOXConfigurationProvider configurationProvider)
		{
			_configuration = configurationProvider.Configuration;
		}

		public string ProtocolName
		{
			get { return "http://correspondence.updatecontrols.net/pox"; }
		}

		public string PeerName
		{
			get { return _configuration.EndpointBase; }
		}

		public void BeginGet(FactTreeMemento pivotTree, FactID pivotId, TimestampID timestamp, Guid clientGuid, Action<FactTreeMemento> callback)
		{
			GetRequest request = new GetRequest
			{
				PivotTree = Translate.MementoToFactTree(pivotTree),
				PivotId = pivotId.key,
				Timestamp = timestamp.Key,
				ClientGuid = clientGuid.ToString()
			};
			WebRequest webRequest = WebRequest.CreateDefault(new Uri(_configuration.EndpointBase));
			webRequest.Method = "POST";
			webRequest.BeginGetRequestStream(a1 =>
			{
				try
				{
					Stream requestStream = webRequest.EndGetRequestStream(a1);
					GetRequestSerializer.Serialize(requestStream, request);
					webRequest.BeginGetResponse(a2 =>
					{
						try
						{
							WebResponse webResponse = webRequest.EndGetResponse(a2);
							Stream responseStream = webResponse.GetResponseStream();
							GetResponse response = (GetResponse)GetResponseSerializer.Deserialize(responseStream);
							callback(Translate.FactTreeToMemento(response.GetResult));
						}
						catch (Exception ex)
						{
							HandleException(ex);
							callback(new FactTreeMemento(pivotTree.DatabaseId, timestamp.Key));
						}
					}, null);
				}
				catch (Exception ex)
				{
					HandleException(ex);
					callback(new FactTreeMemento(pivotTree.DatabaseId, timestamp.Key));
				}
			}, null);
		}

		public void BeginPost(FactTreeMemento messageBody, Guid clientGuid, Action<bool> callback)
		{
			PostRequest request = new PostRequest
			{
				MessageBody = Translate.MementoToFactTree(messageBody),
				ClientGuid = clientGuid.ToString()
			};
			WebRequest webRequest = WebRequest.CreateDefault(new Uri(_configuration.EndpointBase));
			webRequest.Method = "POST";
			webRequest.BeginGetRequestStream(a1 =>
			{
				try
				{
					Stream requestStream = webRequest.EndGetRequestStream(a1);
					PostRequestSerializer.Serialize(requestStream, request);
					webRequest.BeginGetResponse(a2 =>
					{
						try
						{
							WebResponse webResponse = webRequest.EndGetResponse(a2);
							Stream responseStream = webResponse.GetResponseStream();
							PostResponse response = (PostResponse)PostResponseSerializer.Deserialize(responseStream);
							callback(true);
						}
						catch (Exception ex)
						{
							HandleException(ex);
							callback(false);
						}
					}, null);
				}
				catch (Exception ex)
				{
					HandleException(ex);
					callback(false);
				}
			}, null);
		}

		public event Action<FactTreeMemento> MessageReceived;

		public IPushSubscription SubscribeForPush(FactTreeMemento pivotTree, FactID pivotId, Guid clientGuid)
		{
			// Push notification is not supported in the desktop version.
			return new NoOpPushSubscription();
		}

		private void HandleException(Exception ex)
		{
			// TODO
		}
	}
}
