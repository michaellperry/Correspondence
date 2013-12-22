using System;

namespace QEDCode.LLOne
{
    public abstract class Rule<TSymbol, T>
    {
        public abstract bool Start(TSymbol symbol);
        public virtual bool Epsilon() { return false; }
        public abstract T Match(TokenStream<TSymbol> tokenStream);

        public static Rule<TSymbol, T> operator |(Rule<TSymbol, T> rule1, Rule<TSymbol, T> rule2)
        {
            return new Rules.RuleOr<TSymbol, T>(rule1, rule2);
        }
    }
}
