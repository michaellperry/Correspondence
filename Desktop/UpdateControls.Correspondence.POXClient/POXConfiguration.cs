using System;

namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
        private string _endpoint;
        private string _channelName;

        public POXConfiguration(string endpoint, string channelName)
        {
            _endpoint = endpoint;
            _channelName = channelName;
        }

		public string Endpoint
		{
			get { return _endpoint; }
		}

        public string ChannelName
        {
            get { return _channelName; }
        }
	}
}
