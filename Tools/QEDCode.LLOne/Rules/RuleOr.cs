using System;

namespace QEDCode.LLOne.Rules
{
    public class RuleOr<TSymbol, T> : Rule<TSymbol, T>
    {
        private Rule<TSymbol, T> _rule1;
        private Rule<TSymbol, T> _rule2;

        public RuleOr(Rule<TSymbol, T> rule1, Rule<TSymbol, T> rule2)
        {
            _rule1 = rule1;
            _rule2 = rule2;
        }

        public override bool Start(TSymbol symbol)
        {
            bool start1 = _rule1.Start(symbol);
            bool start2 = _rule2.Start(symbol);
            if (start1 && start2)
                throw new ParserException(string.Format("The grammar is ambiguous for symbol {0}. It is not LALR(1).", symbol), 0);
            return start1 || start2;
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            if (_rule1.Start(tokenStream.Lookahead.Symbol))
                return _rule1.Match(tokenStream);
            else
                return _rule2.Match(tokenStream);
        }
    }
}
