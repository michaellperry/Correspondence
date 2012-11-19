using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Strategy;
using UpdateControls.Correspondence.Mementos;

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
            publishCondition.Accept(this);
            return _result;
        }

        public void VisitAnd(Condition left, Condition right)
        {
            left.Accept(this);
            if (_result)
                right.Accept(this);
        }

        public void VisitSimple(bool isEmpty, Queries.QueryDefinition subQuery)
        {
            bool any = _storageStrategy.QueryForIds(subQuery, _startingId).Any();
            _result = (any && !isEmpty || !any && isEmpty);
        }
    }
}
