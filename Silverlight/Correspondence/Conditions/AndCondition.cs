﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Correspondence.Conditions
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

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            AndCondition that = obj as AndCondition;
            if (that == null)
                return false;
            return this._left.Equals(that._left) && this._right.Equals(that._right);
        }

        public override int GetHashCode()
        {
            return _left.GetHashCode() * 37 + _right.GetHashCode();
        }
    }
}
