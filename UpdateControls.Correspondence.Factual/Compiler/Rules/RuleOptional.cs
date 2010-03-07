using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleOptional<T> : Rule<T>
    {
        private Rule<T> _rule;
        private T _defaultValue;

        public RuleOptional(Rule<T> rule, T defaultValue)
        {
            _rule = rule;
            _defaultValue = defaultValue;
        }

        public override bool Start(Symbol symbol)
        {
            return true;
        }

        public override T Match(TokenStream tokenStream)
        {
            if (_rule.Start(tokenStream.Lookahead.Symbol))
                return _rule.Match(tokenStream);
            else
                return _defaultValue;
        }
    }
}
