using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [DataContract(Namespace = Constants.Namespace)]
    public class GetResult
    {
        [DataMember]
        public FactTree FactTree { get; set; }

        [DataMember]
        public long Timestamp { get; set; }
    }
}
