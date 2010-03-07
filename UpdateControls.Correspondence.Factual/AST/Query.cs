using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Query : FactMember
    {
        private string _factName;
        private QueryTail _tail;

        public Query(string name, string factName, QueryTail tail, int lineNumber)
            : base(name, lineNumber)
        {
            _factName = factName;
            _tail = tail;
        }

        public string FactName
        {
            get { return _factName; }
        }

        public IEnumerable<Set> Sets
        {
            get { return _tail.Sets; }
        }
    }
}
