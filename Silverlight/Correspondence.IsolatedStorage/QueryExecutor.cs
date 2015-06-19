using System.Collections.Generic;
using System.Linq;
using Correspondence.Conditions;
using Correspondence.Mementos;
using Correspondence.Queries;
using Correspondence.Strategy;
using System;
using Correspondence.Data;

namespace Correspondence.IsolatedStorage
{
    internal class QueryExecutor : IConditionVisitor
    {
        private IEnumerable<long> _match;
        private HistoricalTree _factTree;
        private Func<RoleMemento, int> _getRoleId;

        public QueryExecutor(HistoricalTree factTree, Func<RoleMemento, int> getRoleId)
        {
            _factTree = factTree;
            _getRoleId = getRoleId;
        }

        public IEnumerable<long> ExecuteQuery(QueryDefinition queryDefinition, long startingId, QueryOptions options)
        {
            // Push.
            IEnumerable<long> pushMatch = _match;

            // Start with the initial id.
            _match = Enumerable.Repeat(startingId, 1);

            // Each step of the query yields a set of matching facts.
            foreach (Join join in queryDefinition.Joins)
            {
                int roleId = _getRoleId(join.Role);
                if (join.Successor)
                {
                    _match = _match.SelectMany(predecessor =>
                        _factTree.GetSuccessorsInRole(predecessor, roleId)
                    );
                }
                else
                {
                    _match = _match.SelectMany(predecessor =>
                        _factTree.GetPredecessorsInRole(predecessor, roleId)
                    );
                }

                // Include the condition, if specified.
                if (join.Condition != null)
                    join.Condition.Accept(this);
            }

            // Pop.
            var result = _match;
            _match = pushMatch;

            if (options != null)
                result = result.Take(options.Limit);

            return result;
        }

        public void VisitAnd(Condition left, Condition right)
        {
            // Apply both filters.
            left.Accept(this);
            right.Accept(this);
        }

        public void VisitSimple(bool isEmpty, QueryDefinition subQuery)
        {
            if (isEmpty)
                _match = _match.Where(f => !ExecuteQuery(subQuery, f, null).Any());
            else
                _match = _match.Where(f => ExecuteQuery(subQuery, f, null).Any());
        }
    }
}
