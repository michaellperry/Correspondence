using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Condition
    {
        private List<Clause> _clauses = new List<Clause>();
        
        public IEnumerable<Clause> Clauses
        {
            get { return _clauses; }
        }

        public Condition AddClause(Clause clause)
        {
            _clauses.Add(clause);
            return this;
        }
    }
}
