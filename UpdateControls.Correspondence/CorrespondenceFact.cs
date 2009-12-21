using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence
{
	/// <summary>
	/// </summary>
	public class CorrespondenceFact
	{
		private FactID _id;
        private IDictionary<RoleMemento, PredecessorBase> _predecessor = new Dictionary<RoleMemento, PredecessorBase>();
        private IDictionary<QueryDefinition, IQueryResult> _resultByQueryDefinition = new Dictionary<QueryDefinition, IQueryResult>();

		private Community _community;

		internal FactID ID
		{
			get { return _id; }
			set { _id = value; }
		}

		internal protected Community Community
		{
			get { return _community; }
		}

		internal void SetCommunity( Community community )
		{
			_community = community;
		}

        internal IEnumerable<RoleMemento> PredecessorRoles
        {
            get { return _predecessor.Keys; }
        }

        internal PredecessorBase GetPredecessor(RoleMemento role)
		{
            PredecessorBase predecessor = _predecessor[role];
			if ( predecessor == null )
				throw new CorrespondenceException( string.Format( "Role not recognized: {0}", role ) );
			return predecessor;
		}

        internal void SetPredecessor(RoleMemento role, PredecessorBase predecessor)
		{
			if ( _predecessor.ContainsKey( role ) )
				throw new CorrespondenceException( string.Format( "Duplicate role: {0}", role ) );
			_predecessor[role] = predecessor;
		}

        internal void AddQueryResult(QueryDefinition queryDefinition, IQueryResult queryResult)
        {
            _resultByQueryDefinition.Add(queryDefinition, queryResult);
        }

        internal void InvalidateQuery(QueryDefinition invalidQuery)
        {
            IQueryResult results;
            if (_resultByQueryDefinition.TryGetValue(invalidQuery, out results))
                results.Invalidate();
        }
	}
}
