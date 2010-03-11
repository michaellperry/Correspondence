using System;

namespace QEDCode.LALROne.Rules
{
    public class RuleOptional<TSymbol, T> : Rule<TSymbol, T>
    {
        private Rule<TSymbol, T> _rule;
        private T _defaultValue;

        public RuleOptional(Rule<TSymbol, T> rule, T defaultValue)
        {
            _rule = rule;
            _defaultValue = defaultValue;
        }

        public override bool Start(TSymbol symbol)
        {
            return true;
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            if (_rule.Start(tokenStream.Lookahead.Symbol))
                return _rule.Match(tokenStream);
            else
                return _defaultValue;
        }
    }
}
