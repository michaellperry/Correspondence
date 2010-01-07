using System;
using UpdateControls.Correspondence.WebService.Contract;

namespace UpdateControls.Correspondence.WebService
{
    public class SynchronizationService : ISynchronizationService
    {
        public FactTree Get(FactTree rootTree, long rootId, long timestamp)
        {
            return new FactTree();
        }

        public void Post(FactTree messageBody)
        {
            throw new NotImplementedException();
        }
    }
}
