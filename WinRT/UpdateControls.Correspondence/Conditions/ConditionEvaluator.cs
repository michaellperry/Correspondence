using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.Conditions
{
    public class ConditionEvaluator : IConditionVisitor
    {
        private IStorageStrategy _storageStrategy;

        private FactID _startingId;
        private bool _result;

        public ConditionEvaluator(IStorageStrategy storageStrategy)
        {
            _storageStrategy = storageStrategy;
        }

        public bool Evaluate(FactID startingId, Condition publishCondition)
        {
            _startingId = startingId;
            _result = true;
            publishCondition.AcceptAsync(this);
            return _result;
        }

        public async Task VisitAndAsync(Condition left, Condition right)
        {
            await left.AcceptAsync(this);
            if (_result)
                await right.AcceptAsync(this);
        }

        public async Task VisitSimpleAsync(bool isEmpty, Queries.QueryDefinition subQuery)
        {
            bool any = (await _storageStrategy.QueryForIdsAsync(subQuery, _startingId)).Any();
            _result = (any && !isEmpty || !any && isEmpty);
        }
    }
}
