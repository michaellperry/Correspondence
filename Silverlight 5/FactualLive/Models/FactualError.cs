using System;
using System.Collections.Generic;

namespace FactualLive.Models
{
    public class FactualError
    {
        private readonly string _message;

        public FactualError(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}
