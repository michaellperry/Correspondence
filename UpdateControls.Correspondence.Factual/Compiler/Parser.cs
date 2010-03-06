using System;
using UpdateControls.Correspondence.Factual.AST;
using System.IO;
using System.Text;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Parser
    {
        private Lexer _lexer;
        private Token _lookahead;

        public Parser(TextReader input)
        {
            _lexer = new Lexer(input)
                .AddSymbol("namespace", Symbol.Namespace)
                .AddSymbol("fact", Symbol.Fact)
                .AddSymbol(".", Symbol.Dot)
                .AddSymbol(";", Symbol.Semicolon)
                .AddSymbol("{", Symbol.OpenBracket)
                .AddSymbol("}", Symbol.CloseBracket)
                ;
        }

        public Namespace Parse()
        {
            Consume();

            Namespace namespaceNode = MatchNamespace();

            while (Lookahead.Symbol == Symbol.Fact)
                namespaceNode.AddFact(MatchFact());

            if (Lookahead.Symbol != Symbol.EndOfFile)
                throw new FactualException("Declare a fact.", Lookahead.LineNumber);

            return namespaceNode;
        }

        private Namespace MatchNamespace()
        {
            Token namespaceToken = Expect(Symbol.Namespace, "Add a 'namespace' declaration.");

            if (Lookahead.Symbol != Symbol.Identifier)
                throw new FactualException("Provide a dotted identifier for the namespace.", Lookahead.LineNumber);
            string namespaceIdentifier = MatchDottedIdentifier();

            Expect(Symbol.Semicolon, "Terminate the namespace declaration with a semicolon.");

            return new Namespace(namespaceIdentifier, namespaceToken.LineNumber);
        }

        private Fact MatchFact()
        {
            Token factToken = Expect(Symbol.Fact, "Declare a fact.");
            Token factNameToken = Expect(Symbol.Identifier, "Provide a name for the fact.");

            Fact fact = new Fact(factNameToken.Value, factNameToken.LineNumber);

            Token openBracketToken = Expect(Symbol.OpenBracket, "Declare members of a fact within brackets.");

            Token closeBracketToken = Expect(Symbol.CloseBracket, "A member must be a field, property, query, or predicate.");

            return fact;
        }

        private string MatchDottedIdentifier()
        {
            StringBuilder result = new StringBuilder();

            Token idenifier = Expect(Symbol.Identifier, "Begin with an identifier.");
            result.Append(idenifier.Value);

            while (Lookahead.Symbol == Symbol.Dot)
            {
                Consume();
                result.Append(".");

                idenifier = Expect(Symbol.Identifier, "A dotted identifier cannot contain other symbols.");
                result.Append(idenifier.Value);
            }

            return result.ToString();
        }

        private Token Lookahead
        {
            get { return _lookahead; }
        }

        private Token Consume()
        {
            Token lastToken = _lookahead;
            _lookahead = _lexer.NextToken();
            return lastToken;
        }

        private Token Expect(Symbol symbol, string errorMessage)
        {
            if (Lookahead.Symbol != symbol)
                throw new FactualException(errorMessage, Lookahead.LineNumber);
            return Consume();
        }
    }
}
