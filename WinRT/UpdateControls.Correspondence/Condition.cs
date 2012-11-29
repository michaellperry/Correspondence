using System;
using System.Collections.Generic;
using System.Linq;
using UpdateControls.Correspondence.Conditions;

namespace UpdateControls.Correspondence
{
	public class Condition
	{
        private List<SimpleCondition> _clauses = new List<SimpleCondition>();

        public static Condition WhereIsEmpty(Query query)
        {
            SimpleCondition clause = new SimpleCondition(true, query);
            Condition condition = new Condition();
            condition._clauses.Add(clause);
            return condition;
        }

        public static Condition WhereIsNotEmpty(Query query)
        {
            SimpleCondition clause = new SimpleCondition(false, query);
            Condition condition = new Condition();
            condition._clauses.Add(clause);
            return condition;
        }

        public AndConditionBuilder And()
        {
            return new AndConditionBuilder(this);
        }

        public List<SimpleCondition> Clauses
        {
            get { return _clauses; }
        }

        public string ToString(string prior)
        {
            return string.Join(" and ", _clauses
                .Select(clause => clause.ToString(prior))
                .ToArray());
        }

        public override string ToString()
        {
            return ToString("this");
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            Condition that = obj as Condition;
            if (that == null)
                return false;

            if (this._clauses.Count != that._clauses.Count)
                return false;

            for (int index = 0; index < this._clauses.Count; ++index)
            {
                if (!object.Equals(this._clauses[index], that._clauses[index]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int index = 0; index < _clauses.Count; ++index)
            {
                hash = hash * 37 + _clauses[index].GetHashCode();
            }
            return hash;
        }
	}
}
