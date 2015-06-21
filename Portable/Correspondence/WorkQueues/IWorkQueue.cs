using System;
using System.Threading.Tasks;

namespace Correspondence.WorkQueues
{
    internal interface IWorkQueue
    {
        void Perform(Func<Task> work);
        Task JoinAsync();
        Exception LastException { get; }
    }
}
