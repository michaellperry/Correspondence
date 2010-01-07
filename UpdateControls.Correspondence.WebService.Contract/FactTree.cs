using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class FactTree
    {
        [DataMember]
        public List<Fact> Facts { get; set; }
    }
}
