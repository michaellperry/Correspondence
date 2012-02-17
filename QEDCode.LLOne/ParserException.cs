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

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
