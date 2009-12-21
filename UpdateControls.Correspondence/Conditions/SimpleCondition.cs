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
    }
}
