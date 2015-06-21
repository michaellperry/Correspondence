using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Correspondence.WorkQueues
{
    internal class SynchronousWorkQueue : IWorkQueue
    {
        public void Perform(Func<Task> work)
        {
            work();
        }

        public Task JoinAsync()
        {
            return Task.FromResult(0);
        }

        public Exception LastException
        {
            get
            {
                return null;
            }
        }
    }
}
