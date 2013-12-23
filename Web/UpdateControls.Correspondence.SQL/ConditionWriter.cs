using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Conditions;
using UpdateControls.Correspondence.Queries;

namespace UpdateControls.Correspondence.SQL
{
    internal class ConditionWriter
    {
        private SQLStorageStrategy _storageStrategy;
        private Session _session;
        private StringBuilder _conditions;
        private List<int> _roleIds;
        private int _depth;
        private string _branchAlias;

        public ConditionWriter(SQLStorageStrategy storageStrategy, Session session, StringBuilder conditions, List<int> roleIds, int depth, string branchAlias)
        {
            _storageStrategy = storageStrategy;
            _session = session;
            _conditions = conditions;
            _roleIds = roleIds;
            _depth = depth;
            _branchAlias = branchAlias;
        }

        public void VisitSimple(bool isEmpty, QueryDefinition subQuery)
        {
            _conditions.Append("\r\nAND ");
            if (isEmpty)
                _conditions.Append("NOT ");
            StringBuilder subConditions = new StringBuilder();
            List<Join> joins = subQuery.Joins.Reverse().ToList();
            Join first = joins[0];
            joins.RemoveAt(0);

            int index = _roleIds.Count;
            _roleIds.Add(_storageStrategy.SaveRole(_session, first.Role));
            string nextAlias = "p" + index;

            string priorAlias;
            string whereAlias;
            if (first.Successor)
            {
                priorAlias = nextAlias + ".FKPredecessorFactID";
                whereAlias = nextAlias + ".FKFactID";
            }
            else
            {
                priorAlias = nextAlias + ".FKFactID";
                whereAlias = nextAlias + ".FKPredecessorFactID";
            }

            if (first.Condition != null)
            {
                ConditionWriter writer = new ConditionWriter(_storageStrategy, _session, subConditions, _roleIds, _depth + 1, whereAlias);
                foreach (var clause in first.Condition.Clauses)
                {
                    writer.VisitSimple(clause.IsEmpty, clause.SubQuery);
                }
            }

            _conditions.AppendFormat("EXISTS (SELECT 1 FROM Predecessor {0} ", nextAlias);

            string factAlias = _storageStrategy.AppendJoins(_session, joins, _conditions, _roleIds, subConditions, _depth + 1, priorAlias);
            _conditions.AppendFormat("\r\nWHERE {0}={1} AND {2}.FKRoleID=@Role{3} ", factAlias, _branchAlias, nextAlias, index);
            _conditions.Append(subConditions);
            _conditions.Append(") ");
        }
    }
}
