using System;

namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
        private readonly string _endpoint;
        private readonly string _channelName;
        private readonly string _apiKey;
        private readonly int _timeoutSeconds;
        
        public POXConfiguration(string endpoint, string channelName, string apiKey) :
            this(endpoint, channelName, apiKey, 0)
        {
        }

        public POXConfiguration(string endpoint, string channelName, string apiKey, int timeoutSeconds)
        {
            _endpoint = endpoint;
            _channelName = channelName;
            _apiKey = apiKey;
            _timeoutSeconds = timeoutSeconds;
        }

		public string Endpoint
		{
			get { return _endpoint; }
		}

        public string ChannelName
        {
            get { return _channelName; }
        }

        public string APIKey
        {
            get { return _apiKey; }
        }

        public int TimeoutSeconds
        {
            get { return _timeoutSeconds; }
        }
    }
}
