using System;

namespace QEDCode.LLOne.Rules
{
    public class RuleMany<TSymbol, TItem, T> : Rule<TSymbol, T>
    {
        private Rule<TSymbol, T> _headerRule;
        private Rule<TSymbol, TItem> _itemRule;
        Func<T, TItem, T> _append;

        public RuleMany(Rule<TSymbol, T> headerRule, Rule<TSymbol, TItem> itemRule, Func<T, TItem, T> append)
        {
            _headerRule = headerRule;
            _itemRule = itemRule;
            _append = append;
        }

        public override bool Start(TSymbol symbol)
        {
            return _headerRule.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T result = _headerRule.Match(tokenStream);

            while (_itemRule.Start(tokenStream.Lookahead.Symbol))
                result = _append(result, _itemRule.Match(tokenStream));

            return result;
        }
    }
}
