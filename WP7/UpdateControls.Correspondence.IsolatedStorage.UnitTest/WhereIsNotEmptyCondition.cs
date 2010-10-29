using UpdateControls.Correspondence.Conditions;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.IsolatedStorage.UnitTest
{
    public class WhereIsNotEmptyCondition : Condition
    {
        private QueryDefinition _queryDefinition;

        public WhereIsNotEmptyCondition(QueryDefinition queryDefinition)
        {
            _queryDefinition = queryDefinition;
        }

        public override string ToString(string prior)
        {
            return string.Empty;
        }

        public override void Accept(IConditionVisitor visitor)
        {
            visitor.VisitSimple(false, _queryDefinition);
        }
    }
}
