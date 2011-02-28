using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    public class Result<TResultType> : IEnumerable<TResultType>, IQueryResult
        where TResultType : CorrespondenceFact
    {
        private CorrespondenceFact _startingPoint;
        private Query _query;
        private QueryOptions _options;
        private List<TResultType> _results;
        private Independent _indResults = new Independent();

        public Result(CorrespondenceFact startingPoint, Query query)
            : this(startingPoint, query, null)
        {
        }

        public Result(CorrespondenceFact startingPoint, Query query, QueryOptions options)
        {
            _startingPoint = startingPoint;
            _query = query;
            _options = options;

            _indResults.GainDependent += new Action(GainDependent);

            startingPoint.AddQueryResult(query.QueryDefinition, this);
        }

        public IEnumerator<TResultType> GetEnumerator()
        {
            lock (this)
            {
                _indResults.OnGet();
                List<TResultType> results = new List<TResultType>(_results);
                return results.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                _indResults.OnGet();
                List<TResultType> results = new List<TResultType>(_results);
                return results.GetEnumerator();
            }
        }

        private void GainDependent()
        {
            // If the results are not cached, load them.
            if (_results == null)
            {
                // Load the results from storage and cache them.
                _results =
                    _startingPoint.InternalCommunity.ExecuteQuery(_query.QueryDefinition, _startingPoint.ID, _options)
                    .Cast<TResultType>()
                    .ToList();
            }
        }

        public void Invalidate()
        {
			lock (this)
			{
			    _indResults.OnSet();
			    UnloadResults();
			}
        }

        private void UnloadResults()
        {
            _results = null;
        }
    }
}
