using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.WorkQueues
{
    internal class SynchronousWorkQueue : IWorkQueue
    {
        public void Perform(Func<Task> work)
        {
            work();
        }

        public Task[] Tasks
        {
            get { return new Task[0]; }
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
