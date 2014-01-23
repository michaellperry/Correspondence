using System;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.WorkQueues
{
    internal interface IWorkQueue
    {
        void Perform(Func<Task> work);
        Task WhenAllAsync();
        Exception LastException { get; }
    }
}
