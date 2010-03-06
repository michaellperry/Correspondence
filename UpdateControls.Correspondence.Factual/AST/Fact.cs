using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Fact
    {
        private int _lineNumber;
        private string _name;

        public Fact(string name, int lineNumber)
        {
            _name = name;
            _lineNumber = lineNumber;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
