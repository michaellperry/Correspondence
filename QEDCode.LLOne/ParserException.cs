using System;
using System.Runtime.Serialization;

namespace QEDCode.LLOne
{
    public class ParserException : Exception
    {
        private int _lineNumber;

        public ParserException(int lineNumber)
        {
            _lineNumber = lineNumber;
        }

        public ParserException(string message, int lineNumber)
            : base(message)
        {
            _lineNumber = lineNumber;
        }

        public ParserException(string message, int lineNumber, Exception innerException)
            : base(message, innerException)
        {
            _lineNumber = lineNumber;
        }

        protected ParserException(SerializationInfo info, StreamingContext context)
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
