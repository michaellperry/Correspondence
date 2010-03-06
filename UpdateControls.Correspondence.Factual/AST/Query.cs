using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Query
    {
        private int _lineNumber;
        private string _name;
        private string _factName;
        private List<Set> _sets = new List<Set>();

        public Query(string name, string factName, int lineNumber)
        {
            _name = name;
            _factName = factName;
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

        public string FactName
        {
            get { return _factName; }
        }

        public IEnumerable<Set> Sets
        {
            get { return _sets; }
        }

        public void AddSet(Set set)
        {
            _sets.Add(set);
        }
    }
}
