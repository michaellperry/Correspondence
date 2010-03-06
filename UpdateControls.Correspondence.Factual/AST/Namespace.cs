using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Namespace
    {
        private int _lineNumber;
        private string _identifier;
        private List<Fact> _facts = new List<Fact>();

        public Namespace(string identifier, int lineNumber)
        {
            _lineNumber = lineNumber;
            _identifier = identifier;
        }

        public void AddFact(Fact fact)
        {
            _facts.Add(fact);
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        public IEnumerable<Fact> Facts
        {
            get { return _facts; }
        }
    }
}
