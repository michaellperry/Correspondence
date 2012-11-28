﻿using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;
using System.Threading;
using System.Threading.Tasks;

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
        private ManualResetEvent _loaded = new ManualResetEvent(false);
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

        public IEnumerable<TResultType> Ensure()
        {
            lock (this)
            {
                _indResults.OnGet();
                LoadResults();
            }
            _loaded.WaitOne();
            lock (this)
            {
                return _results;
            }
        }

        public TransientDisputable<TResultType, TValue> AsTransientDisputable<TValue>(Func<TResultType, TValue> selector)
        {
            return new TransientDisputable<TResultType, TValue>(this, selector);
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
                Task<List<CorrespondenceFact>> queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                    _query.QueryDefinition, _startingPoint.ID, _options);
                if (queryTask.IsCompleted)
                {
                    _results = queryTask.Result
                        .OfType<TResultType>()
                        .ToList();
                    SetState(State.Loaded);
                }
                else
                {
                    SetState(State.Loading);
                    queryTask.ContinueWith(ExecuteQueryCompleted);
                }
            }
        }

        private void ExecuteQueryCompleted(Task<List<CorrespondenceFact>> queryTask)
        {
            lock (this)
            {
                _indResults.OnSet();
                _results = queryTask.Result
                    .OfType<TResultType>()
                    .ToList();
                if (_state == State.Loading)
                {
                    SetState(State.Loaded);
                }
                else if (_state == State.Invalidated)
                {
                    if (_indResults.HasDependents)
                    {
                        // Load the results from storage and cache them.
                        queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                            _query.QueryDefinition, _startingPoint.ID, _options);
                        if (queryTask.IsCompleted)
                        {
                            _indResults.OnSet();
                            _results = queryTask.Result
                                .OfType<TResultType>()
                                .ToList();
                            SetState(State.Loaded);
                        }
                        else
                        {
                            SetState(State.Loading);
                            queryTask.ContinueWith(ExecuteQueryCompleted);
                        }
                    }
                    else
                    {
                        SetState(State.Unloaded);
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
                        Task<List<CorrespondenceFact>> queryTask = _startingPoint.InternalCommunity.ExecuteQueryAsync(
                            _query.QueryDefinition, _startingPoint.ID, _options);
                        if (queryTask.IsCompleted)
                        {
                            _indResults.OnSet();
                            _results = queryTask.Result
                                .OfType<TResultType>()
                                .ToList();
                            SetState(State.Loaded);
                        }
                        else
                        {
                            SetState(State.Loading);
                            queryTask.ContinueWith(ExecuteQueryCompleted);
                        }
                    }
                    else
                    {
                        SetState(State.Unloaded);
                    }
                }
                else if (_state == State.Loading)
                {
                    SetState(State.Invalidated);
                }
			}
        }

        private void SetState(State newState)
        {
            bool wasLoaded = _state == State.Loaded;
            bool isLoaded = newState == State.Loaded;

            _state = newState;
            if (wasLoaded && !isLoaded)
                _loaded.Reset();
            else if (!wasLoaded && isLoaded)
                _loaded.Set();
        }
    }

    public class TransientDisputable<TFact, TValue>
        where TFact : CorrespondenceFact
    {
        private readonly Result<TFact> _result;
        private readonly Func<TFact, TValue> _selector;
        private readonly TValue _value;

        public TransientDisputable(Result<TFact> result, Func<TFact, TValue> selector)
        {
            _result = result;
            _selector = selector;
        }

        private TransientDisputable(TValue value)
        {
            _value = value;
        }

        public TValue Value
        {
            get { return _result != null ? _result.Select(_selector).FirstOrDefault() : _value; }
        }

        public bool InConflict
        {
            get { return _result.Count() > 1; }
        }

        public IEnumerable<TValue> Candidates
        {
            get { return _result.Select(_selector); }
        }

        public Disputable<TValue> Ensure()
        {
            return _result.Ensure().Select(_selector).AsDisputable();
        }

        public static implicit operator TValue(TransientDisputable<TFact, TValue> disputable)
        {
            return disputable.Value;
        }

        public static implicit operator TransientDisputable<TFact, TValue>(TValue value)
        {
            return new TransientDisputable<TFact, TValue>(value);
        }
    }
}