using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class FactRole
    {
        [DataMember]
        public int RoleId { get; set; }

        [DataMember]
        public int DeclaringTypeId { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public int TargetTypeId { get; set; }

        [DataMember]
        public bool IsPivot { get; set; }
    }
}
