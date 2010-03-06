using System;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class TokenStream
    {
        private Lexer _lexer;
        private Token _lookahead;

        public TokenStream(Lexer lexer)
        {
            _lexer = lexer;
        }

        public Token Lookahead
        {
            get { return _lookahead; }
        }

        public Token Consume()
        {
            Token lastToken = _lookahead;
            _lookahead = _lexer.NextToken();
            return lastToken;
        }

        public Token Expect(Symbol symbol, string errorMessage)
        {
            if (Lookahead.Symbol != symbol)
                throw new FactualException(errorMessage, Lookahead.LineNumber);
            return Consume();
        }
    }
}
