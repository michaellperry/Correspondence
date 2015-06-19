using System;
using System.Collections.Generic;
using System.Linq;
using Correspondence.Mementos;
using Correspondence.Queries;

namespace Correspondence
{
    class QueryInverter
    {
        private CorrespondenceFactType _priorType;
        private Action<CorrespondenceFactType, QueryDefinition> _recordInverse;
        private QueryDefinition _inverse = new QueryDefinition();

        public QueryInverter(CorrespondenceFactType priorType, Action<CorrespondenceFactType, QueryDefinition> recordInverse)
        {
            _priorType = priorType;
            _recordInverse = recordInverse;
        }

        public void InvertQuery(IEnumerable<Join> joins)
        {
            Condition condition = null;

            foreach (Join join in joins)
            {
                // Ensure that the joins interlock.
                if (join.Successor)
                {
                    // TODO: Allow inheritance.
                    //if (!_priorType.Equals(join.Role.TargetType))
                    //    throw new CorrespondenceException(string.Format("Successor join to {0} is not allowable from {1}.", join.Role, _priorType));
                    _priorType = join.Role.DeclaringType;
                }
                else
                {
                    // Record the partial inverse query because we are stepping up to predecessors.
                    if (_inverse.Joins.Any())
                        _recordInverse(_priorType, _inverse.Copy());

                    // TODO: Allow inheritance.
                    //if (!_priorType.Equals(join.Role.TargetType))
                    //    throw new CorrespondenceException(string.Format("Successor join to {0} is not allowable from {1}.", join.Role, _priorType));
                    _priorType = join.Role.TargetType;
                }

                // Build up the inverse.
                _inverse.PrependInverse(join, condition);

                // Dive into the sub query.
                HandleCondition(join.Condition);

                condition = join.Condition;
            }

            // Record the inverse query for invalidation.
            _recordInverse(_priorType, _inverse);
        }

        public void HandleCondition(Condition condition)
        {
            if (condition != null)
            {
                foreach (var clause in condition.Clauses)
                {
                    VisitSimpleAsync(clause.IsEmpty, clause.SubQuery);
                }
            }
        }

        private void VisitSimpleAsync(bool isEmpty, QueryDefinition subQuery)
        {
            // Push
            CorrespondenceFactType pushPriorType = _priorType;
            QueryDefinition pushQueryDefinition = _inverse;
            _inverse = _inverse.Copy();

            InvertQuery(subQuery.Joins);

            // Pop
            _priorType = pushPriorType;
            _inverse = pushQueryDefinition;
        }
    }
}
