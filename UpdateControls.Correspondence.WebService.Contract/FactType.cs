using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class FactType
    {
        [DataMember]
        string TypeName { get; set; }

        [DataMember]
        int Version { get; set; }
    }
}
