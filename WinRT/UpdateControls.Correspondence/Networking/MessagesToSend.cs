using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;

namespace UpdateControls.Correspondence.Networking
{
    public class MessagesToSend
    {
        public TimestampID Timestamp { get; set; }
        public FactTreeMemento MessageBodies { get; set; }
        public List<UnpublishMemento> UnpublishedMessages { get; set; }
    }
}
