using System;
using UpdateControls.Correspondence.Factual.AST;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
                .AddSymbol("property", Symbol.Property)
                .AddSymbol("string", Symbol.String)
                .AddSymbol("int", Symbol.Int)
                .AddSymbol("float", Symbol.Float)
                .AddSymbol("char", Symbol.Char)
                .AddSymbol("date", Symbol.Date)
                .AddSymbol("time", Symbol.Time)
                .AddSymbol(".", Symbol.Dot)
                .AddSymbol(";", Symbol.Semicolon)
                .AddSymbol("{", Symbol.OpenBracket)
                .AddSymbol("}", Symbol.CloseBracket)
                .AddSymbol("?", Symbol.Question)
                .AddSymbol("*", Symbol.Asterisk)
                ;
        }

        public Namespace Parse()
        {
            Consume();

            Namespace namespaceNode = MatchNamespace();

            while (StartOfFact())
                namespaceNode.AddFact(MatchFact());

            if (Lookahead.Symbol != Symbol.EndOfFile)
                throw new FactualException("Declare a fact.", Lookahead.LineNumber);

            return namespaceNode;
        }

        private Namespace MatchNamespace()
        {
            Token namespaceToken = Expect(Symbol.Namespace, "Add a 'namespace' declaration.");

            if (!StartOfDottedIdentifier())
                throw new FactualException("Provide a dotted identifier for the namespace.", Lookahead.LineNumber);
            string namespaceIdentifier = MatchDottedIdentifier();

            Expect(Symbol.Semicolon, "Terminate the namespace declaration with a semicolon.");

            return new Namespace(namespaceIdentifier, namespaceToken.LineNumber);
        }

        private bool StartOfDottedIdentifier()
        {
            return Lookahead.Symbol == Symbol.Identifier;
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

        private bool StartOfFact()
        {
            return Lookahead.Symbol == Symbol.Fact;
        }

        private Fact MatchFact()
        {
            Token factToken = Expect(Symbol.Fact, "Declare a fact.");
            Token factNameToken = Expect(Symbol.Identifier, "Provide a name for the fact.");

            Fact fact = new Fact(factNameToken.Value, factNameToken.LineNumber);

            Token openBracketToken = Expect(Symbol.OpenBracket, "Declare members of a fact within brackets.");

            while (true)
            {
                if (StartOfType())
                {
                    DataType type = MatchType();
                    Token nameToken = Expect(Symbol.Identifier, "Provide a name for the field or query.");
                    Expect(Symbol.Semicolon, "Terminate a field with a semicolon.");
                    fact.AddField(new DataMember(type.LineNumber, nameToken.Value, type));
                }
                else if (StartOfProperty())
                    fact.AddProperty(MatchProperty());
                else
                    break;
            }

            Token closeBracketToken = Expect(Symbol.CloseBracket, "A member must be a field, property, query, or predicate.");

            return fact;
        }

        private bool StartOfProperty()
        {
            return Lookahead.Symbol == Symbol.Property;
        }

        private DataMember MatchProperty()
        {
            Token propertyToken = Expect(Symbol.Property, "Begin a property with the keyword \"property\".");
            DataType type = MatchType();
            Token nameToken = Expect(Symbol.Identifier, "Provide a name for the field or query.");
            Expect(Symbol.Semicolon, "Terminate a property with a semicolon.");
            return new DataMember(propertyToken.LineNumber, nameToken.Value, type);
        }

        private bool StartOfType()
        {
            return
                StartOfNativeType() ||
                Lookahead.Symbol == Symbol.Identifier;
        }

        private DataType MatchType()
        {
            if (StartOfNativeType())
                return MatchNativeType();
            else
                throw new FactualException("Declare a native type or fact type.", Lookahead.LineNumber);
        }

        private static Dictionary<Symbol, NativeType> _nativeTypeBySymbol = new Dictionary<Symbol, NativeType>()
        {
            { Symbol.String, NativeType.String },
            { Symbol.Int, NativeType.Int },
            { Symbol.Float, NativeType.Float },
            { Symbol.Char, NativeType.Char },
            { Symbol.Date, NativeType.Date },
            { Symbol.Time, NativeType.Time }
        };

        private bool StartOfNativeType()
        {
            return _nativeTypeBySymbol.ContainsKey(Lookahead.Symbol);
        }

        private DataTypeNative MatchNativeType()
        {
            Token typeToken = Consume();
            return new DataTypeNative(_nativeTypeBySymbol[typeToken.Symbol], MatchCardinality(), typeToken.LineNumber);
        }

        private Cardinality MatchCardinality()
        {
            if (Lookahead.Symbol == Symbol.Question)
            {
                Consume();
                return Cardinality.Optional;
            }
            else if (Lookahead.Symbol == Symbol.Asterisk)
            {
                Consume();
                return Cardinality.Many;
            }
            else
                return Cardinality.One;
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
