using System;
using System.Collections.Generic;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.NetworkSimulator
{
    public class SimulatedServer : SimulatedMachine, ICommunicationStrategy2
    {
        public string ProtocolName
        {
            get { return "simulation"; }
        }

        public string PeerName
        {
            get { return "server"; }
        }

        public void Post(FactTreeMemento messageBody)
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
