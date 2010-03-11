using System;

namespace QEDCode.LLOne.Rules
{
    public class RuleSeparated<TSymbol, T, TItem> : Rule<TSymbol, T>
    {
        private TSymbol _separator;
        private Rule<TSymbol, TItem> _itemRule;
        Func<TItem, T> _begin;
        Func<T, TItem, T> _append;

        public RuleSeparated(Rule<TSymbol, TItem> itemRule, TSymbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            _separator = separator;
            _itemRule = itemRule;
            _begin = begin;
            _append = append;
        }

        public override bool Start(TSymbol symbol)
        {
            return _itemRule.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T result = _begin(_itemRule.Match(tokenStream));

            while (tokenStream.Lookahead.Symbol.Equals(_separator))
            {
                tokenStream.Consume();
                result = _append(result, _itemRule.Match(tokenStream));
            }

            return result;
        }
    }
}
