using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedClient : ICommunicationStrategy
    {
        private SimulatedNetwork _network;

        public SimulatedClient(SimulatedNetwork network)
        {
            _network = network;

            network.AttachClient(this);
        }

        public SimulatedClient Post<FactType>(Func<FactType, string> factToUrl)
            where FactType : CorrespondenceFact
        {
            return this;
        }

        public void AttachFactRepository(IFactRepository repository)
        {
        }

        public void Synchronize()
        {
        }
    }
}
