using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class Predecessor
    {
        [DataMember]
        public int RoleId { get; set; }

        [DataMember]
        public long PredecessorId { get; set; }
    }
}
