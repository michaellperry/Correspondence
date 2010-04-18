using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Predicate : FactMember
    {
        private List<Clause> _clauses = new List<Clause>();

        public Predicate(string name, int lineNumber)
            : base(name, lineNumber)
        {
        }

        public Predicate AddClause(Clause condition)
        {
            _clauses.Add(condition);
            return this;
        }

        public IEnumerable<Clause> Clauses
        {
            get { return _clauses; }
        }
    }
}
