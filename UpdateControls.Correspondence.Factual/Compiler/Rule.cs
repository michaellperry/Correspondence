using System;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public abstract class Rule<T>
    {
        public abstract bool Start(Symbol symbol);
        public abstract T Match(TokenStream tokenStream);

        public static Rule<T> operator |(Rule<T> rule1, Rule<T> rule2)
        {
            return new Rules.RuleOr<T>(rule1, rule2);
        }
    }
}
