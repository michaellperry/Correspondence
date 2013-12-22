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

    public class RuleManyHeadless<TSymbol, TItem, T> : Rule<TSymbol, T>
    {
        private readonly T _start;
        private readonly Rule<TSymbol, TItem> _itemRule;
        private readonly Func<T, TItem, T> _append;

        public RuleManyHeadless(T start, Rule<TSymbol, TItem> itemRule, Func<T, TItem, T> append)
        {
            _start = start;
            _itemRule = itemRule;
            _append = append;
        }

        public override bool Start(TSymbol symbol)
        {
            return _itemRule.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            T result = _start;

            while (_itemRule.Start(tokenStream.Lookahead.Symbol))
                result = _append(result, _itemRule.Match(tokenStream));

            return result;
        }

        public override bool Epsilon()
        {
            return true;
        }
    }
}
