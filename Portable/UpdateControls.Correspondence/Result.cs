using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Strategy;
using System.Threading;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Threading;

namespace UpdateControls.Correspondence
{
    static class ResultScheduler
    {
        private static object _mutex = new object();
        private static List<IQueryResult> _resultsLoading = new List<IQueryResult>();
        private static List<IQueryResult> _resultsLoaded = new List<IQueryResult>();

        internal static void BeginLoading(IQueryResult result)
        {
            lock (_mutex)
            {
                _resultsLoading.Add(result);
            }
        }

        internal static void EndLoading(IQueryResult result)
        {
            List<IQueryResult> resultsToSignal = null;
            lock (_mutex)
            {
                if (_resultsLoading.Remove(result))
                {
                    _resultsLoaded.Add(result);
                    if (!_resultsLoading.Any())
                    {
                        resultsToSignal = _resultsLoaded;
                        _resultsLoaded = new List<IQueryResult>();
                    }
                }
            }

            if (resultsToSignal != null)
            {
                foreach (var r in resultsToSignal)
                    r.Signal();
            }
        }
    }

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
        private AsyncManualResetEvent _loaded = new AsyncManualResetEvent();
        private Independent _indResults = new Independent();
        private Func<TResultType> _getUnloadedInstance;
        private Func<TResultType> _getNullInstance;

        public Result(CorrespondenceFact startingPoint, Query query, Func<TResultType> getUnloadedInstance, Func<TResultType> getNullInstance)
            : this(startingPoint, query, getUnloadedInstance, getNullInstance, null)
        {
        }

        public Result(CorrespondenceFact startingPoint, Query query, Func<TResultType> getUnloadedInstance, Func<TResultType> getNullInstance, QueryOptions options)
        {
            _startingPoint = startingPoint;
            _query = query;
            _getUnloadedInstance = getUnloadedInstance;
            _getNullInstance = getNullInstance;
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

        public async Task<Result<TResultType>> EnsureAsync()
        {
            lock (this)
            {
                _indResults.OnGet();
                LoadResults();
                if (_state == State.Loaded)
                    return this;
            }
            await _loaded.WaitAsync();
            return this;
        }

        public TransientDisputable<TResultType, TValue> AsTransientDisputable<TValue>(Func<TResultType, TValue> selector)
        {
            return new TransientDisputable<TResultType, TValue>(this, selector);
        }

        public TResultType Top()
        {
            lock (this)
            {
                _indResults.OnGet();
                LoadResults();
                if (_state == State.Loaded)
                    return _results.FirstOrDefault() ?? _getNullInstance();
                else
                    return _getUnloadedInstance();
            }
        }

        public bool IsLoaded
        {
            get
            {
                lock (this)
                {
                    _indResults.OnGet();
                    LoadResults();
                    return _state == State.Loaded;
                }
            }
        }

        public TResultType GetNullInstance()
        {
            return _getNullInstance();
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
            if (_state == State.Unloaded && _startingPoint.IsLoaded)
            {
                if (_startingPoint.IsNull)
                {
                    SetState(State.Loaded);
                }
                else
                {
                    StartLoading();
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
                        StartLoading();
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

        public void Signal()
        {
            lock (this)
            {
                _indResults.OnSet();
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

        private void StartLoading()
        {
            // Load the results from storage and cache them.
            _startingPoint.Community.Perform(async delegate
            {
                lock (this)
                {
                    SetState(State.Loading);
                    ResultScheduler.BeginLoading(this);
                }

                var facts = await _startingPoint.InternalCommunity.ExecuteQueryAsync(
                    _query.QueryDefinition, _startingPoint.ID, _options);

                lock (this)
                {
                    ResultScheduler.EndLoading(this);
                    _results = facts
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
                            StartLoading();
                        }
                        else
                        {
                            SetState(State.Unloaded);
                        }
                    }
                }
            });
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
            get { return _result != null ? _selector(_result.Top()) : _value; }
        }

        public bool InConflict
        {
            get { return _result.Count() > 1; }
        }

        public IEnumerable<TValue> Candidates
        {
            get { return _result.Select(_selector); }
        }

        public async Task<Disputable<TValue>> EnsureAsync()
        {
            IEnumerable<TFact> facts = await _result.EnsureAsync();
            return facts.Select(_selector).AsDisputable(() => _selector(_result.GetNullInstance()));
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
