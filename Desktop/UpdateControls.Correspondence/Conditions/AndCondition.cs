using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence.Conditions
{
    public class AndCondition : Condition
    {
        private Condition _left;
        private Condition _right;

        public AndCondition(Condition left, Condition right)
        {
            _left = left;
            _right = right;
        }

        public override string ToString(string prior)
        {
            return string.Format("{0} and {1}", _left.ToString(prior), _right.ToString(prior));
        }

        public override void Accept(IConditionVisitor visitor)
        {
            visitor.VisitAnd(_left, _right);
        }
    }
}
