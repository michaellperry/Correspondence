using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QEDCode.LLOne.Rules
{
    public class ForwardRule<TSymbol, T> : Rule<TSymbol, T>
    {
        private Rule<TSymbol, T> _rule;

        public void Set(Rule<TSymbol, T> value)
        {
            _rule = value;
        }

        public override bool Start(TSymbol symbol)
        {
            return _rule.Start(symbol);
        }

        public override T Match(TokenStream<TSymbol> tokenStream)
        {
            return _rule.Match(tokenStream);
        }
    }
}
