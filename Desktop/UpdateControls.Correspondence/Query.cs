using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence
{
    public class Query
    {
        private QueryDefinition _queryDefinition = new QueryDefinition();

        public Query JoinSuccessors(Role role)
        {
            _queryDefinition.AddJoin(true, role.RoleMemento, null);
            return this;
        }

		public Query JoinSuccessors(Role role, Condition condition)
		{
			_queryDefinition.AddJoin(true, role.RoleMemento, condition);
			return this;
		}

        public Query JoinPredecessors(Role role)
        {
            _queryDefinition.AddJoin(false, role.RoleMemento, null);
            return this;
        }

        public Query JoinPredecessors(Role role, Condition condition)
        {
            _queryDefinition.AddJoin(false, role.RoleMemento, condition);
            return this;
        }

        public QueryDefinition QueryDefinition
        {
            get { return _queryDefinition; }
        }
    }
}
