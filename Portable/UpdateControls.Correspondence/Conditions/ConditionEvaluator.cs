using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.Conditions
{
    public class ConditionEvaluator
    {
        private IStorageStrategy _storageStrategy;

        private FactID _startingId;

        public ConditionEvaluator(IStorageStrategy storageStrategy)
        {
            _storageStrategy = storageStrategy;
        }

        public async Task<bool> EvaluateAsync(FactID startingId, Condition publishCondition)
        {
            _startingId = startingId;
            foreach (var clause in publishCondition.Clauses)
            {
                bool any = (await _storageStrategy.QueryForIdsAsync(clause.SubQuery, _startingId)).Any();
                if (!any && !clause.IsEmpty || any && clause.IsEmpty)
                    return false;
            }

            return true;
        }
    }
}
