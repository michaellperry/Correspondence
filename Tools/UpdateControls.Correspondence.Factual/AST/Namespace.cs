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
        private string _version;
        private List<Fact> _facts = new List<Fact>();

        public Namespace(string identifier, int lineNumber, List<Header> headers, string strength)
        {
            _lineNumber = lineNumber;
            _identifier = identifier;
            _strength = strength;

            _version = headers
                .Where(h => h.Name == "version")
                .Select(h => h.Parameter)
                .FirstOrDefault();
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

        public string Version
        {
            get { return _version; }
        }

        public IEnumerable<Fact> Facts
        {
            get { return _facts; }
        }
    }
}
