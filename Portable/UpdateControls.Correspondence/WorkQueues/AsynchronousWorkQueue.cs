using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Fields;

namespace UpdateControls.Correspondence.WorkQueues
{
    internal class AsynchronousWorkQueue : IWorkQueue
    {
        private Independent<Exception> _lastException = new Independent<Exception>();
        private List<Token> _tokens = new List<Token>();

        public void Perform(Func<Task> work)
        {
            Token token = CreateToken();
            token.Task = Task.Run(async delegate
            {
                try
                {
                    LastException = null;
                    await work();
                }
                catch (Exception x)
                {
                    LastException = x;
                }
                finally
                {
                    DestroyToken(token);
                }
            });
        }

        public async Task WhenAllAsync()
        {
            Task[] tasks;
            lock (this)
            {
                tasks = _tokens.Select(t => t.Task).ToArray();
            }
            await Task.WhenAll(tasks);
        }

        private Token CreateToken()
        {
            lock (this)
            {
                var token = new Token();
                _tokens.Add(token);
                return token;
            }
        }

        private void DestroyToken(Token token)
        {
            lock (this)
            {
                _tokens.Remove(token);
            }
        }

        public Exception LastException
        {
            get
            {
                lock (this)
                {
                    return _lastException;
                }
            }
            private set
            {
                lock (this)
                {
                    _lastException.Value = value;
                }
            }
        }
    }
}
