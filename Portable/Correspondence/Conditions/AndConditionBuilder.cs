
namespace Correspondence.Conditions
{
    public class AndConditionBuilder
    {
        private Condition _condition;

        public AndConditionBuilder(Condition condition)
        {
            _condition = condition;
        }

        public Condition IsEmpty(Query query)
        {
            _condition.Clauses.Add(new Clause(true, query));
            return _condition;
        }

        public Condition IsNotEmpty(Query query)
        {
            _condition.Clauses.Add(new Clause(false, query));
            return _condition;
        }
    }
}
