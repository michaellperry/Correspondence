using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleMany<TItem, T>: Rule<T>
    {
        private Rule<T> _headerRule;
        private Rule<TItem> _itemRule;
        Func<T, TItem, T> _append;

        public RuleMany(Rule<T> headerRule, Rule<TItem> itemRule, Func<T, TItem, T> append)
        {
            _headerRule = headerRule;
            _itemRule = itemRule;
            _append = append;
        }

        public override bool Start(Symbol symbol)
        {
            return _headerRule.Start(symbol);
        }

        public override T Match(TokenStream tokenStream)
        {
            T result = _headerRule.Match(tokenStream);

            while (_itemRule.Start(tokenStream.Lookahead.Symbol))
                result = _append(result, _itemRule.Match(tokenStream));

            return result;
        }
    }
}
