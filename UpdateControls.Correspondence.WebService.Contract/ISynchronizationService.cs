using System;
using System.ServiceModel;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [ServiceContract(Namespace=Constants.Namespace)]
    public interface ISynchronizationService
    {
        [OperationContract]
        FactTree Get(FactTree pivotTree, long pivotId, long timestamp, Guid clientGuid);

        [OperationContract]
        void Post(FactTree messageBody, Guid clientGuid);
    }
}
