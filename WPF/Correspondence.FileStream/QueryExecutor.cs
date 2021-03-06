using System.Collections.Generic;
using System.Linq;
using Correspondence.Conditions;
using Correspondence.Mementos;
using Correspondence.Queries;
using Correspondence.Strategy;
using System;
using Correspondence.Data;
using System.Threading.Tasks;

namespace Correspondence.FileStream
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
                    List<long> successors = new List<long>();
                    foreach (var predecessor in match)
                    {
                        successors.AddRange(
                            _factTree.GetSuccessorsInRole(predecessor, roleId));
                    }
                    match = successors;
                }
                else
                {
                    List<long> predecessors = new List<long>();
                    foreach (var successor in match)
                    {
                        predecessors.AddRange(
                            _factTree.GetPredecessorsInRole(successor, roleId));
                    }
                    match = predecessors;
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
