﻿using System;
using UpdateControls.Correspondence.Factual.AST;
using System.Text;
using System.Collections.Generic;
using UpdateControls.Correspondence.Factual.Compiler.Rules;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class Parser
    {
        private TokenStream _tokenStream;
        private Rule<PathRelative> _relativePathRule;
        private Rule<Namespace> _namespaceRule;

        public Parser(System.IO.TextReader input)
        {
            Lexer _lexer = new Lexer(input)
                .AddSymbol("namespace", Symbol.Namespace)
                .AddSymbol("fact", Symbol.Fact)
                .AddSymbol("property", Symbol.Property)
                .AddSymbol("this", Symbol.This)
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
                .AddSymbol(":", Symbol.Colon)
                .AddSymbol("=", Symbol.Equal)
                ;

            _tokenStream = new TokenStream(_lexer);
            var dottedIdentifier = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                identifier => new StringBuilder().Append(identifier.Value),
                (stringBuilder, identifier) => stringBuilder.Append(".").Append(identifier.Value));
            _relativePathRule = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                segment => new PathRelative().AddSegment(segment.Value),
                (path, segment) => path.AddSegment(segment.Value));
            _namespaceRule = Sequence(
                Terminal(Symbol.Namespace),
                dottedIdentifier, "Provide a dotted identifier for the namespace.",
                Terminal(Symbol.Semicolon), "Terminate the namespace declaration with a semicolon.",
                (namespaceToken, identifier, ignored) => new Namespace(identifier.ToString(), namespaceToken.LineNumber));
        }

        public static Rule<Token> Terminal(Symbol expectedSymbol)
        {
            return new RuleTerminal(expectedSymbol);
        }

        public static Rule<T> Separated<T, TItem>(Rule<TItem> itemRule, Symbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            return new RuleSeparated<T, TItem>(itemRule, separator, begin, append);
        }

        private static Rule<T> Sequence<T1, T2, T3, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Func<T1, T2, T3, T> reduce)
        {
            return new RuleSequence3<T1, T2, T3, T>(rule1, rule2, error2, rule3, error3, reduce);
        }

        public Namespace Parse()
        {
            Consume();

            if (!StartOfNamespace())
                throw new FactualException("Add a 'namespace' declaration.", Lookahead.LineNumber);
            Namespace namespaceNode = MatchNamespace();

            while (StartOfFact())
                namespaceNode.AddFact(MatchFact());

            if (Lookahead.Symbol != Symbol.EndOfFile)
                throw new FactualException("Declare a fact.", Lookahead.LineNumber);

            return namespaceNode;
        }

        private bool StartOfNamespace()
        {
            return _namespaceRule.Start(Lookahead.Symbol);
        }

        private Namespace MatchNamespace()
        {
            return _namespaceRule.Match(_tokenStream);
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
                    if (Lookahead.Symbol == Symbol.OpenBracket)
                    {
                        Consume();

                        DataTypeFact factType = type as DataTypeFact;
                        if (factType == null)
                            throw new FactualException("A query must return a fact type, not a native type.", type.LineNumber);
                        if (factType.Cardinality != Cardinality.Many)
                            throw new FactualException("A query must return multiple results.", factType.LineNumber);

                        Query query = new Query(nameToken.Value, factType.FactName, factType.LineNumber);

                        while (StartOfSet())
                            query.AddSet(MatchSet());
                        fact.AddQuery(query);

                        Expect(Symbol.CloseBracket, "A query must contain sets.");
                    }
                    else
                    {
                        Expect(Symbol.Semicolon, "Terminate a field with a semicolon.");
                        fact.AddField(new DataMember(type.LineNumber, nameToken.Value, type));
                    }
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

        private bool StartOfSet()
        {
            return Lookahead.Symbol == Symbol.Identifier;
        }

        private Set MatchSet()
        {
            Token factNameToken = Expect(Symbol.Identifier, "Declare a fact type for the set.");
            Token setNameToken = Expect(Symbol.Identifier, "Provide a name for the set.");
            Expect(Symbol.Colon, "Use a colon to separate the set name from its definition.");
            Path leftPath = MatchPath();
            Expect(Symbol.Equal, "Separate paths with an equal sign.");
            Path rightPath = MatchPath();
            return new Set(setNameToken.Value, factNameToken.Value, leftPath, rightPath, factNameToken.LineNumber);
        }

        private Path MatchPath()
        {
            if (StartOfRelativePath())
            {
                return MatchRelativePath();
            }
            else
            {
                Token thisToken = Expect(Symbol.This, "Provide either a relative path or \"this\".");
                return new PathAbsolute();
            }
        }

        private bool StartOfRelativePath()
        {
            return _relativePathRule.Start(Lookahead.Symbol);
        }

        private PathRelative MatchRelativePath()
        {
            return _relativePathRule.Match(_tokenStream);
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
            {
                Token factNameToken = Expect(Symbol.Identifier, "Declare a native type or fact type.");
                return new DataTypeFact(factNameToken.Value, MatchCardinality(), factNameToken.LineNumber);
            }
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
            get { return _tokenStream.Lookahead; }
        }

        private Token Consume()
        {
            return _tokenStream.Consume();
        }

        private Token Expect(Symbol symbol, string errorMessage)
        {
            return _tokenStream.Expect(symbol, errorMessage);
        }
    }
}
