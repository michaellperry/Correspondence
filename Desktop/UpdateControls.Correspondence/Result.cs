using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence
{
    public class Result<TResultType> : IEnumerable<TResultType>, IQueryResult
        where TResultType : CorrespondenceFact
    {
        private enum State
        {
            Unloaded,
            Loading,
            Loaded,
            Invalidated
        }

        private CorrespondenceFact _startingPoint;
        private Query _query;
        private QueryOptions _options;
        private List<TResultType> _results = new List<TResultType>();
        private State _state = State.Unloaded;
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

            startingPoint.AddQueryResult(query.QueryDefinition, this);
        }

        public IEnumerator<TResultType> GetEnumerator()
        {
            lock (this)
            {
                _indResults.OnGet();
                LoadResults();
                return _results.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                _indResults.OnGet();
                LoadResults();
                return _results.GetEnumerator();
            }
        }

        private void LoadResults()
        {
            // If the results are not cached, load them.
            if (_state == State.Unloaded)
            {
                // Load the results from storage and cache them.
                QueryTask queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                    _query.QueryDefinition, _startingPoint.ID, _options);
                if (queryTask.CompletedSynchronously)
                {
                    _results = queryTask.Result
                        .OfType<TResultType>()
                        .ToList();
                    _state = State.Loaded;
                }
                else
                {
                    _state = State.Loading;
                    queryTask.ContinueWith(ExecuteQueryCompleted);
                }
            }
        }

        private void ExecuteQueryCompleted(QueryTask queryTask)
        {
            lock (this)
            {
                _indResults.OnSet();
                _results = queryTask.Result
                    .OfType<TResultType>()
                    .ToList();
                if (_state == State.Loading)
                {
                    _state = State.Loaded;
                }
                else if (_state == State.Invalidated)
                {
                    if (_indResults.HasDependents)
                    {
                        // Load the results from storage and cache them.
                        queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                            _query.QueryDefinition, _startingPoint.ID, _options);
                        if (queryTask.CompletedSynchronously)
                        {
                            _indResults.OnSet();
                            _results = queryTask.Result
                                .OfType<TResultType>()
                                .ToList();
                            _state = State.Loaded;
                        }
                        else
                        {
                            _state = State.Loading;
                            queryTask.ContinueWith(ExecuteQueryCompleted);
                        }
                    }
                    else
                    {
                        _state = State.Unloaded;
                    }
                }
            }
        }

        public void Invalidate()
        {
			lock (this)
			{
                if (_state == State.Loaded)
                {
                    if (_indResults.HasDependents)
                    {
                        // Load the results from storage and cache them.
                        QueryTask queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                            _query.QueryDefinition, _startingPoint.ID, _options);
                        if (queryTask.CompletedSynchronously)
                        {
                            _indResults.OnSet();
                            _results = queryTask.Result
                                .OfType<TResultType>()
                                .ToList();
                            _state = State.Loaded;
                        }
                        else
                        {
                            _state = State.Loading;
                            queryTask.ContinueWith(ExecuteQueryCompleted);
                        }
                    }
                    else
                    {
                        _state = State.Unloaded;
                    }
                }
                else if (_state == State.Loading)
                {
                    _state = State.Invalidated;
                }
			}
        }
    }
}
