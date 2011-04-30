using System;

namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
        private readonly string _endpoint;
        private readonly string _channelName;
        private readonly string _akiKey;
        
        public POXConfiguration(string endpoint, string channelName, string apiKey)
        {
            _endpoint = endpoint;
            _channelName = channelName;
            _akiKey = apiKey;
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
            get { return _akiKey; }
        }
    }
}
