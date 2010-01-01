using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedNetwork
    {
        private List<SimulatedClient> _clients = new List<SimulatedClient>();
        private SimulatedServer _server;

        public void AttachClient(SimulatedClient client)
        {
            _clients.Add(client);
        }

        public void AttachServer(SimulatedServer server)
        {
            if (_server != null)
                throw new NetworkSimulatorException("The network simulator can only handle one simulated server.");

            _server = server;
        }

        public void Synchronize()
        {
            if (_server == null)
                throw new NetworkSimulatorException("The network simulator must have one simulated server.");

            bool any;
            do
            {
                any = false;
                foreach (SimulatedClient client in _clients)
                    if (client.Synchronize())
                        any = true;
            } while (any);
        }

        public void SendToServer(string path, MessageBodyMemento messageBody)
        {
            _server.Receive(path, messageBody);
        }

        public MessageBodyMemento GetFromServer(string path, TimestampID timestamp)
        {
            return _server.Get(path, timestamp);
        }
    }
}
