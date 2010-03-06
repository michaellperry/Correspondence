using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.Factual
{
    public class FactualException : Exception
    {
        private int _lineNumber;

        public FactualException(int lineNumber)
        {
            _lineNumber = lineNumber;
        }

        public FactualException(string message, int lineNumber)
            : base(message)
        {
            _lineNumber = lineNumber;
        }

        public FactualException(string message, int lineNumber, Exception innerException)
            : base(message, innerException)
        {
            _lineNumber = lineNumber;
        }

        protected FactualException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _lineNumber = info.GetInt32("LineNumber");
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
