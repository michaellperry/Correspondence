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
        private static XmlSerializerNamespaces DefaultNamespaces = DefineNamespaces();
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
			WebRequest webRequest = WebRequest.Create(new Uri(_configuration.Endpoint));
			webRequest.Method = "POST";
			webRequest.BeginGetRequestStream(a1 =>
			{
				try
				{
					Stream requestStream = webRequest.EndGetRequestStream(a1);
                    GetRequestSerializer.Serialize(requestStream, request, DefaultNamespaces);
					webRequest.BeginGetResponse(a2 =>
					{
						try
						{
							WebResponse webResponse = webRequest.EndGetResponse(a2);
							Stream responseStream = webResponse.GetResponseStream();
							GetResponse response = (GetResponse)GetResponseSerializer.Deserialize(responseStream);
							callback(Translate.FactTreeToMemento(response.FactTree), new TimestampID(response.FactTree.DatabaseId, response.Timestamp));
						}
						catch (Exception ex)
						{
							HandleException(ex);
                            callback(new FactTreeMemento(pivotTree.DatabaseId), new TimestampID(pivotTree.DatabaseId, timestamp.Key));
						}
					}, null);
				}
				catch (Exception ex)
				{
					HandleException(ex);
                    callback(new FactTreeMemento(pivotTree.DatabaseId), new TimestampID(pivotTree.DatabaseId, timestamp.Key));
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
			WebRequest webRequest = WebRequest.Create(new Uri(_configuration.Endpoint));
			webRequest.Method = "POST";
			webRequest.BeginGetRequestStream(a1 =>
			{
				try
				{
					Stream requestStream = webRequest.EndGetRequestStream(a1);
                    PostRequestSerializer.Serialize(requestStream, request, DefaultNamespaces);
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

		private void HandleException(Exception ex)
		{
			// TODO
		}

        private static XmlSerializerNamespaces DefineNamespaces()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("c", "http://correspondence.updatecontrols.com/pox/1.0");
            return namespaces;
        }
    }
}
