using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : ICommunicationStrategy
    {
        private SimulatedNetwork _network;

        public SimulatedServer(SimulatedNetwork network)
        {
            _network = network;

            network.AttachServer(this);
        }

        public void AttachFactRepository(IFactRepository repository)
        {
        }
    }
}
