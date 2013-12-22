using System;

namespace QEDCode.LLOne
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
            get
            {
                if (_lookahead == null)
                    _lookahead = _lexer.NextToken();
                return _lookahead;
            }
        }

        public Token<TSymbol> Consume()
        {
            Token<TSymbol> lastToken = Lookahead;
            _lookahead = null;
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
