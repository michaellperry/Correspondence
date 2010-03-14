using System;

namespace QEDCode.LLOne
{
    public class ParserError
    {
        private string _message;
        private int _lineNumber;

        public ParserError(string message, int lineNumber)
        {
            _message = message;
            _lineNumber = lineNumber;
        }

        public string Message
        {
            get { return _message; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
