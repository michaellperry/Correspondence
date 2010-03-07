using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleTranslate<TFrom, T> : Rule<T>
    {
        private Rule<TFrom> _ruleFrom;
        private Func<TFrom, T> _translation;

        public RuleTranslate(Rule<TFrom> ruleFrom, Func<TFrom, T> translation)
        {
            _ruleFrom = ruleFrom;
            _translation = translation;
        }

        public override bool Start(Symbol symbol)
        {
            return _ruleFrom.Start(symbol);
        }

        public override T Match(TokenStream tokenStream)
        {
            return _translation(_ruleFrom.Match(tokenStream));
        }
    }
}
