using System;

namespace UpdateControls.Correspondence.Factual.Compiler.Rules
{
    public class RuleTerminal<T> : Rule<T>
    {
        private Symbol _expectedSymbol;
        private Func<Token, T> _resolve;

        public RuleTerminal(Symbol expectedSymbol, Func<Token, T> resolve)
        {
            _expectedSymbol = expectedSymbol;
            _resolve = resolve;
        }

        public override bool Start(Symbol symbol)
        {
            return symbol == _expectedSymbol;
        }

        public override T Match(TokenStream tokenStream)
        {
            return _resolve(tokenStream.Consume());
        }
    }
}
