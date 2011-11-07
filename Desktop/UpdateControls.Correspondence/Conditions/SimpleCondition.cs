using System;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.Conditions
{
    public class SimpleCondition : Condition
    {
		private bool _isEmpty;
		private QueryDefinition _subQuery;

        internal SimpleCondition(bool isEmpty, Query subQuery)
		{
			_isEmpty = isEmpty;
			_subQuery = subQuery.QueryDefinition;
		}

        public override string ToString(string prior)
        {
            return (_isEmpty ? "empty " : "not empty ") + _subQuery.ToString(prior);
        }

        public override void Accept(IConditionVisitor visitor)
        {
            visitor.VisitSimple(_isEmpty, _subQuery);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            SimpleCondition that = obj as SimpleCondition;
            if (that == null)
                return false;
            return this._isEmpty.Equals(that._isEmpty) && this._subQuery.Equals(that._subQuery);
        }

        public override int GetHashCode()
        {
            return _subQuery.GetHashCode() * 2 + (_isEmpty ? 1 : 0);
        }
    }
}
