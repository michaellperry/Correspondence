
namespace Correspondence.Queries
{
    class QueryInvalidator
    {
        private QueryDefinition _targetFacts;
        private QueryDefinition _invalidQuery;

        public QueryInvalidator(QueryDefinition targetFacts, QueryDefinition invalidQuery)
        {
            _targetFacts = targetFacts;
            _invalidQuery = invalidQuery;
        }

        public QueryDefinition TargetFacts
        {
            get { return _targetFacts; }
        }

        public QueryDefinition InvalidQuery
        {
            get { return _invalidQuery; }
        }

		public override string ToString()
		{
			return string.Format("({0}) -> ({1})", _targetFacts, _invalidQuery);
		}
    }
}
