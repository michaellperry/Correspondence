using System;
using System.ServiceModel;

namespace UpdateControls.Correspondence.WebService.Contract
{
    [ServiceContract(Namespace=Constants.Namespace)]
    public interface ISynchronizationService
    {
        [OperationContract]
        FactTree Get(FactTree rootTree, long rootId, long timestamp);

        [OperationContract]
        void Post(FactTree messageBody);
    }
}
