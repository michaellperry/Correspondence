using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Conditions;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;

namespace UpdateControls.Correspondence.Memory
{
    public class QueryExecutor : IConditionVisitor
    {
        private IEnumerable<IdentifiedMemento> _facts;

        private IEnumerable<IdentifiedMemento> _match;
        public QueryExecutor(IEnumerable<IdentifiedMemento> facts)
        {
            _facts = facts;
        }

        public IEnumerable<IdentifiedMemento> ExecuteQuery(QueryDefinition queryDefinition, FactID startingId, QueryOptions options)
        {
            // Push.
            IEnumerable<IdentifiedMemento> pushMatch = _match;

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
                    finalJoin.Condition.Accept(this);
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
                _match = _match.Where(f => !ExecuteQuery(subQuery, f.Id, null).Any());
            else
                _match = _match.Where(f => ExecuteQuery(subQuery, f.Id, null).Any());
        }
    }
}
