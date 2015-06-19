using System;

namespace Correspondence.Factual.AST
{
    public abstract class FactMember
    {
        private string _name;
        private int _lineNumber;

        public FactMember(string name, int lineNumber)
        {
            _name = name;
            _lineNumber = lineNumber;
        }

        public string Name
        {
            get { return _name; }
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }
    }
}
