using System;

namespace QEDCode.LALROne
{
    public class TokenStream<TSymbol>
    {
        private Lexer<TSymbol> _lexer;
        private Token<TSymbol> _lookahead;

        public TokenStream(Lexer<TSymbol> lexer)
        {
            _lexer = lexer;
        }

        public Token<TSymbol> Lookahead
        {
            get { return _lookahead; }
        }

        public Token<TSymbol> Consume()
        {
            Token<TSymbol> lastToken = _lookahead;
            _lookahead = _lexer.NextToken();
            return lastToken;
        }

        public Token<TSymbol> Expect(TSymbol symbol, string errorMessage)
        {
            if (!Lookahead.Symbol.Equals(symbol))
                throw new ParserException(errorMessage, Lookahead.LineNumber);
            return Consume();
        }
    }
}
