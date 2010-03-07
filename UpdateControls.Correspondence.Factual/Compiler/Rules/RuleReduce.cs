using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleReduce<TFrom, T> : Rule<T>
    {
        private Rule<TFrom> _ruleFrom;
        private Func<TFrom, T> _reduce;

        public RuleReduce(Rule<TFrom> ruleFrom, Func<TFrom, T> reduce)
        {
            _ruleFrom = ruleFrom;
            _reduce = reduce;
        }

        public override bool Start(Symbol symbol)
        {
            return _ruleFrom.Start(symbol);
        }

        public override T Match(TokenStream tokenStream)
        {
            return _reduce(_ruleFrom.Match(tokenStream));
        }
    }
}
