using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.IsolatedStorage.UnitTest
{
    public class WhereIsEmptyCondition : Condition
    {
        private QueryDefinition _queryDefinition;

        public WhereIsEmptyCondition(QueryDefinition queryDefinition)
        {
            _queryDefinition = queryDefinition;
        }

        public override string ToString(string prior)
        {
            return string.Empty;
        }

        public override void Accept(Conditions.IConditionVisitor visitor)
        {
            visitor.VisitSimple(true, _queryDefinition);
        }
    }
}
