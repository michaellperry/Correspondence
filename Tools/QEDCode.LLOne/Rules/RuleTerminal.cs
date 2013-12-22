using System;

namespace QEDCode.LLOne.Rules
{
    public class RuleTerminal<TSymbol> : Rule<TSymbol, Token<TSymbol>>
    {
        private TSymbol _expectedSymbol;

        public RuleTerminal(TSymbol expectedSymbol)
        {
            _expectedSymbol = expectedSymbol;
        }

        public override bool Start(TSymbol symbol)
        {
            return symbol.Equals(_expectedSymbol);
        }

        public override Token<TSymbol> Match(TokenStream<TSymbol> tokenStream)
        {
            return tokenStream.Consume();
        }
    }
}
