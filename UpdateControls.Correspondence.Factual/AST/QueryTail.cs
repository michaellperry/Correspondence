using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class QueryTail
    {
        private List<Set> _sets = new List<Set>();

        public IEnumerable<Set> Sets
        {
            get { return _sets; }
        }

        public QueryTail AddSet(Set set)
        {
            _sets.Add(set);
            return this;
        }
    }
}
