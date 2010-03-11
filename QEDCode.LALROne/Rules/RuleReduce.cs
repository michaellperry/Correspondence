using System;

namespace QEDCode.LALROne.Rules
{
    public class RuleReduce<TSymbol, TFrom, T> : Rule<TSymbol, T>
    {
        private Rule<TSymbol, TFrom> _ruleFrom;
        private Func<TFrom, T> _reduce;

        public RuleReduce(Rule<TSymbol, TFrom> ruleFrom, Func<TFrom, T> reduce)
        {
            _ruleFrom = ruleFrom;
            _reduce = reduce;
        }

        public override bool Start(TSymbol symbol)
        {
            return _ruleFrom.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            return _reduce(_ruleFrom.Match(tokenStream));
        }
    }
}
