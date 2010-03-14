using System;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Error
    {
        private string _message;
        private int _lineNumber;

        public Error(string message, int lineNumber)
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
