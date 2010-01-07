using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class Predecessor
    {
        [DataMember]
        public FactType DeclaringType { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public FactType TargetType { get; set; }

        [DataMember]
        public long PredecessorId { get; set; }
    }
}
