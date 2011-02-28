using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence
{
    public class Query
    {
        private QueryDefinition _queryDefinition = new QueryDefinition();

        public Query JoinSuccessors(RoleBase role)
        {
            _queryDefinition.AddJoin(true, role.RoleMemento, null);
            return this;
        }

		public Query JoinSuccessors(RoleBase role, Condition condition)
		{
			_queryDefinition.AddJoin(true, role.RoleMemento, condition);
			return this;
		}

        public Query JoinPredecessors(RoleBase role)
        {
            _queryDefinition.AddJoin(false, role.RoleMemento, null);
            return this;
        }

        public Query JoinPredecessors(RoleBase role, Condition condition)
        {
            _queryDefinition.AddJoin(false, role.RoleMemento, condition);
            return this;
        }

        internal QueryDefinition QueryDefinition
        {
            get { return _queryDefinition; }
        }
    }
}
