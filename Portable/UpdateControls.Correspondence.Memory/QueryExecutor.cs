using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpdateControls.Correspondence.Conditions;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class QueryExecutor
    {
        private IEnumerable<IdentifiedFactMemento> _facts;

        private IEnumerable<IdentifiedFactMemento> _match;
        public QueryExecutor(IEnumerable<IdentifiedFactMemento> facts)
        {
            _facts = facts;
        }

        public IEnumerable<IdentifiedFactMemento> ExecuteQuery(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            // Push.
            IEnumerable<IdentifiedFactMemento> pushMatch = _match;

            // Start with the initial id.
            _match = _facts.Where(m => m.Id.Equals(startingId));

            // Each step of the query yields a set of matching facts.
            foreach (Join join in queryDefinition.Joins)
            {
                // Declare a new variable to hold this join instance.
                // The variable itself is included within the closure of tha lambda expression.
                // Without this, all of the lambda expressions will use the last join of the
                // query definition, because they will all include the same variable.
                Join finalJoin = join;
                if (finalJoin.Successor)
                {
                    // Find all of the objects for which this object is a predecessor
                    // in the given role.
                    _match = _match.SelectMany(predecessor => _facts
                        .Where(successor =>
                            successor.Memento.Predecessors.Any(pm =>
                                pm.Role.Equals(finalJoin.Role) &&
                                pm.ID.Equals(predecessor.Id))
                        )
                    );
                }
                else
                {
                    // Find all predecessors in the given role.
                    _match = _match.SelectMany(successor => _facts
                        .Where(predecessor =>
                            successor.Memento.Predecessors.Any(pm =>
                                pm.Role.Equals(finalJoin.Role) &&
                                pm.ID.Equals(predecessor.Id))
                        )
                    );
                }

                // Include the condition, if specified.
                if (finalJoin.Condition != null)
                    HandleCondition(finalJoin.Condition);
            }

            // Pop.
            var result = _match;
            _match = pushMatch;

            if (options != null)
                result = result.Take(options.Limit);

            return result;
        }

        private void HandleCondition(Condition condition)
        {
            foreach (var clause in condition.Clauses)
            {
                HandleClause(clause.IsEmpty, clause.SubQuery);
            }
        }

        private void HandleClause(bool isEmpty, QueryDefinition subQuery)
        {
            if (isEmpty)
                _match = _match.Where(f => !ExecuteQuery(subQuery, f.Id, null).Any());
            else
                _match = _match.Where(f => ExecuteQuery(subQuery, f.Id, null).Any());
        }
    }
}
