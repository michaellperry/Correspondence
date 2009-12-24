using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Correspondence.Mementos;
using System;

namespace UpdateControls.Correspondence.Queries
{
    public class QueryDefinition
    {
        private List<Join> _joins;

        public QueryDefinition()
        {
            _joins = new List<Join>();
        }

        public void AddJoin(bool successor, RoleMemento role, Condition condition)
        {
            _joins.Add(new Join(successor, role, condition));
        }

		public void PrependInverse(Join join, Condition condition)
        {
            _joins.Insert(0, new Join(!join.Successor, join.Role, condition));
		}

		public QueryDefinition Copy()
		{
			QueryDefinition copy = new QueryDefinition();
			copy._joins.AddRange(_joins);
            return copy;
        }

        public IEnumerable<Join> Joins
        {
            get { return _joins; }
        }

        public bool CanExecuteFromMemento
        {
            get
            {
                // We can execute single predecessor joins with no conditions directly on the memento.
                return _joins.Count == 1 && !_joins[0].Successor && _joins[0].Condition == null;
            }
        }

        public List<FactID> ExecuteFromMemento(FactMemento memento)
        {
            RoleMemento predecessorRole = _joins[0].Role;
            return memento.Predecessors
                .Where(p => p.Role.Equals(predecessorRole))
                .Select(p => p.ID)
                .ToList();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            QueryDefinition that = (QueryDefinition)obj;
            if (this._joins.Count != that._joins.Count)
                return false;
            for (int i = 0; i < this._joins.Count; i++)
                if (!this._joins[i].Equals(that._joins[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return _joins.Aggregate(0, (hash, join) => hash * 37 + join.GetHashCode());
        }

        public string ToString(string prior)
        {
			StringBuilder result = new StringBuilder();
			bool first = true;
			foreach (Join join in _joins)
			{
				if (first)
					result.AppendFormat("{0} {{",
						(join.Successor ? join.Role.TargetType : join.Role.DeclaringType)
						.TypeName.Split('.').Last());
				else
					result.Append("  ");
				first = false;

				result.Append(join.ToString(prior));
				prior = (join.Successor ? join.Role.DeclaringType : join.Role.TargetType)
					.TypeName.Split('.').Last().ToLower();
			}
			result.Append("}");
			return result.ToString();
		}

        public override string ToString()
        {
            return ToString("this");
        }
    }
}
