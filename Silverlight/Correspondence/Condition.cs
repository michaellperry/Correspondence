using System;
using Correspondence.Conditions;

namespace Correspondence
{
	public abstract class Condition
	{
		public static Condition WhereIsEmpty(Query query)
		{
			return new SimpleCondition(true, query);
		}

		public static Condition WhereIsNotEmpty(Query query)
		{
			return new SimpleCondition(false, query);
		}

        public AndConditionBuilder And()
        {
            return new AndConditionBuilder(this);
        }

        public abstract string ToString(string prior);

        public override string ToString()
        {
            return ToString("this");
        }

        public abstract void Accept(IConditionVisitor visitor);
	}
}
