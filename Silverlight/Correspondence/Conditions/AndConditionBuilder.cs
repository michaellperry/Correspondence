
namespace Correspondence.Conditions
{
    public class AndConditionBuilder
    {
        private Condition _left;

        public AndConditionBuilder(Condition left)
        {
            _left = left;
        }

        public Condition IsEmpty(Query query)
        {
            return new AndCondition(_left, Condition.WhereIsEmpty(query));
        }

        public Condition IsNotEmpty(Query query)
        {
            return new AndCondition(_left, Condition.WhereIsNotEmpty(query));
        }
    }
}
