using System;

namespace UpdateControls.Correspondence.POXClient
{
	public class POXConfiguration
	{
        private readonly string _endpoint;
        private readonly string _channelName;
        private readonly string _username;
        private readonly string _password;
        
        public POXConfiguration(string endpoint, string channelName, string username, string password)
        {
            _endpoint = endpoint;
            _channelName = channelName;
            _username = username;
            _password = password;
        }

		public string Endpoint
		{
			get { return _endpoint; }
		}

        public string ChannelName
        {
            get { return _channelName; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }
    }
}
