using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class FactTree
    {
        [DataMember]
        public long DatabaseId { get; set; }

        [DataMember]
        public List<FactType> Types { set; get; }

        [DataMember]
        public List<FactRole> Roles { get; set; }

        [DataMember]
        public List<Fact> Facts { get; set; }
    }
}
