using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class FactType
    {
        [DataMember]
        public string TypeName { get; set; }

        [DataMember]
        public int Version { get; set; }
    }
}
