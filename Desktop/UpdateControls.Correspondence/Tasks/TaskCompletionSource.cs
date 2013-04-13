using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Tasks
{
    public class TaskCompletionSource<T>
    {
        public Task<T> Task { get; set; }

        public void SetResult(T result)
        {
            throw new NotImplementedException();
        }
    }
}
