using System;

namespace Correspondence.Tasks
{
    public class TaskCompletionSource<T>
    {
        public Task<T> Task { get; private set; }

        public TaskCompletionSource()
        {
            Task = new Task<T>();
        }

        public void SetResult(T result)
        {
            Task.Complete(result);
        }
    }
}
