using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Predicate : FactMember
    {
        private ConditionModifier _existence;
        private List<Set> _sets = new List<Set>();

        public Predicate(string name, ConditionModifier existence, int lineNumber)
            : base(name, lineNumber)
        {
            _existence = existence;
        }

        public ConditionModifier Existence
        {
            get { return _existence; }
        }

        public IEnumerable<Set> Sets
        {
            get { return _sets; }
        }

        public Predicate AddSet(Set set)
        {
            _sets.Add(set);
            return this;
        }
    }
}
