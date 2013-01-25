using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Conditions;
using UpdateControls.Correspondence.Mementos;
using UpdateControls.Correspondence.Queries;
using UpdateControls.Correspondence.Strategy;
using System;
using UpdateControls.Correspondence.Data;
using System.Threading.Tasks;

namespace UpdateControls.Correspondence.FileStream
{
    internal class QueryExecutor
    {
        private HistoricalTree _factTree;
        private Func<RoleMemento, Task<int>> _getRoleId;

        public QueryExecutor(HistoricalTree factTree, Func<RoleMemento, Task<int>> getRoleId)
        {
            _factTree = factTree;
            _getRoleId = getRoleId;
        }

        public async Task<List<long>> ExecuteQuery(QueryDefinition queryDefinition, long startingId, QueryOptions options)
        {
            // Start with the initial id.
            List<long> match = new List<long> { startingId };

            // Each step of the query yields a set of matching facts.
            foreach (Join join in queryDefinition.Joins)
            {
                int roleId = await _getRoleId(join.Role);
                if (join.Successor)
                {
                    match = await Task.Run(() =>
                        match.SelectMany(predecessor =>
                            _factTree.GetSuccessorsInRole(predecessor, roleId))
                        .ToList()
                    );
                }
                else
                {
                    match = await Task.Run(() =>
                        match.SelectMany(successor =>
                            _factTree.GetPredecessorsInRole(successor, roleId))
                        .ToList()
                    );
                }

                // Include the condition, if specified.
                if (join.Condition != null)
                {
                    foreach (var clause in join.Condition.Clauses)
                    {
                        List<long> exists = new List<long>();
                        foreach (long factId in match)
                        {
                            List<long> subMatch = await ExecuteQuery(clause.SubQuery, factId, null);
                            if (subMatch.Any())
                                exists.Add(factId);
                        }
                        if (clause.IsEmpty)
                            match.RemoveAll(r => exists.Contains(r));
                        else
                            match.RemoveAll(r => !exists.Contains(r));
                    }
                }
            }

            if (options != null)
                match = match.Take(options.Limit).ToList();

            return match;
        }
    }
}
