using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : SimulatedMachine
    {
        private SimulatedNetwork _network;

        public SimulatedServer(SimulatedNetwork network)
        {
            _network = network;

            network.AttachServer(this);
        }

        public void Receive(FactTreeMemento messageBody)
        {
            ReceiveMessage(messageBody);
        }

        public FactTreeMemento Get(FactTreeMemento rootTree, FactID remoteRootId, TimestampID timestamp)
        {
            FactID localRootId = FindExistingFact(remoteRootId, rootTree);
            IEnumerable<FactID> recentMessages = _repository.LoadRecentMessages(localRootId, timestamp);
            FactTreeMemento messageBody = new FactTreeMemento();
            foreach (FactID recentMessage in recentMessages)
                AddToFactTree(messageBody, recentMessage);

            return messageBody;
        }
    }
}
