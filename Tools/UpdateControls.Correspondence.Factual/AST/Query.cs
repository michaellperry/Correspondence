using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Query : FactMember
    {
        private string _factName;
        private List<Set> _sets = new List<Set>();

        public Query(string name, string factName, int lineNumber)
            : base(name, lineNumber)
        {
            _factName = factName;
        }

        public string FactName
        {
            get { return _factName; }
        }

        public IEnumerable<Set> Sets
        {
            get { return _sets; }
        }

        public Query AddSet(Set set)
        {
            _sets.Add(set);
            return this;
        }
    }
}
