using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleSeparated<T, TItem> : Rule<T>
    {
        private Symbol _separator;
        private Rule<TItem> _itemRule;
        Func<TItem, T> _begin;
        Func<T, TItem, T> _append;

        public RuleSeparated(Rule<TItem> itemRule, Symbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            _separator = separator;
            _itemRule = itemRule;
            _begin = begin;
            _append = append;
        }

        public override bool Start(Symbol symbol)
        {
            return _itemRule.Start(symbol);
        }

        public override T Match(TokenStream tokenStream)
        {
            T result = _begin(_itemRule.Match(tokenStream));

            while (tokenStream.Lookahead.Symbol == _separator)
            {
                tokenStream.Consume();
                result = _append(result, _itemRule.Match(tokenStream));
            }

            return result;
        }
    }
}
