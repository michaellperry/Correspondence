using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class Fact
    {
        [DataMember]
        public long FactId { get; set; }

        [DataMember]
        public FactType FactType { get; set; }

        [DataMember]
        public byte[] Data { get; set; }

        [DataMember]
        public List<Predecessor> Predecessors { get; set; }
    }
}
