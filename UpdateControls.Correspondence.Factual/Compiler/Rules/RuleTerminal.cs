using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleTerminal : Rule<Token>
    {
        private Symbol _expectedSymbol;

        public RuleTerminal(Symbol expectedSymbol)
        {
            _expectedSymbol = expectedSymbol;
        }

        public override bool Start(Symbol symbol)
        {
            return symbol == _expectedSymbol;
        }

        public override Token Match(TokenStream tokenStream)
        {
            return tokenStream.Consume();
        }
    }
}
