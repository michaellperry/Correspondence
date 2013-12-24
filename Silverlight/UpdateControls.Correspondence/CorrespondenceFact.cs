using System.Collections.Generic;
using UpdateControls.Correspondence.Mementos;
using System;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Tasks;

namespace UpdateControls.Correspondence
{
	/// <summary>
	/// </summary>
	public abstract class CorrespondenceFact
	{
        private FactID _id;
        private IDictionary<RoleMemento, PredecessorBase> _predecessor = new Dictionary<RoleMemento, PredecessorBase>();
        private IDictionary<QueryDefinition, List<IQueryResult>> _resultByQueryDefinition = new Dictionary<QueryDefinition, List<IQueryResult>>();

		private Community _community;

        protected internal abstract CorrespondenceFactType GetCorrespondenceFactType();

        private bool _isLoaded = true;
        private bool _isNull = false;

        protected Task<CorrespondenceFact> _loadedTask;

        internal void SetLoadedTask(Task<CorrespondenceFact> loadedTask)
        {
            _loadedTask = loadedTask;
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            protected set { _isLoaded = value; }
        }

        public bool IsNull
        {
            get { return _isNull; }
            protected set { _isNull = value; }
        }

        internal FactID ID
		{
			get { return _id; }
			set { _id = value; }
		}

		protected ICommunity Community
		{
			get { return _community; }
		}

        internal Community InternalCommunity
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
            PredecessorBase predecessor;
            if (!_predecessor.TryGetValue(role, out predecessor))
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
			List<IQueryResult> results;
			if (!_resultByQueryDefinition.TryGetValue(queryDefinition, out results))
			{
				results = new List<IQueryResult>();
				_resultByQueryDefinition.Add(queryDefinition, results);
			}
			results.Add(queryResult);
        }

        internal void InvalidateQuery(QueryDefinition invalidQuery)
        {
            List<IQueryResult> results;
            if (_resultByQueryDefinition.TryGetValue(invalidQuery, out results))
				foreach (IQueryResult result in results)
					result.Invalidate();
        }
    }
}
