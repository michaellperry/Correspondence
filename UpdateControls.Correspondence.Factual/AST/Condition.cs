using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Clause
    {
        private ConditionModifier _existence;
        private List<Set> _sets = new List<Set>();
        private int _lineNumber;

        public Clause(ConditionModifier existence, int lineNumber)
        {
            _existence = existence;
            _lineNumber = lineNumber;
        }

        public ConditionModifier Existence
        {
            get { return _existence; }
        }

        public IEnumerable<Set> Sets
        {
            get { return _sets; }
        }

        public Clause AddSet(Set set)
        {
            _sets.Add(set);
            return this;
        }
    }
}
