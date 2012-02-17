using System;
using System.Runtime.Serialization;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class CompilerException : Exception
    {
        private int _lineNumber;

        public CompilerException(int lineNumber)
        {
            _lineNumber = lineNumber;
        }

        public CompilerException(string message, int lineNumber)
            : base(message)
        {
            _lineNumber = lineNumber;
        }

        public CompilerException(string message, int lineNumber, Exception innerException)
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
