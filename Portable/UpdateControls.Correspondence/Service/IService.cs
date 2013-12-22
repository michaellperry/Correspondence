using System.Collections.Generic;

namespace UpdateControls.Correspondence.Service
{
    public interface IService<TWorkItem>
    {
        IEnumerable<TWorkItem> Queue { get; }
        void Process(List<TWorkItem> queue);
    }
}
