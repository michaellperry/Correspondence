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
        private string _strength;
        private List<Fact> _facts = new List<Fact>();

        public Namespace(string identifier, int lineNumber, string strength)
        {
            _lineNumber = lineNumber;
            _identifier = identifier;
            _strength = strength;
        }

        public Namespace AddFact(Fact fact)
        {
            _facts.Add(fact);
            return this;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        public string Strength
        {
            get { return _strength; }
        }
        public IEnumerable<Fact> Facts
        {
            get { return _facts; }
        }
    }
}
