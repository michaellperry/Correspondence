using System;
using UpdateControls.Correspondence.Factual.AST;
using System.Text;
using System.Collections.Generic;
using UpdateControls.Correspondence.Factual.Compiler.Rules;
using System.Diagnostics;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class RuleFact : Rule<Fact>
    {
        private Rule<DataType> _typeRule;
        private Rule<Set> _setRule;
        private Rule<DataMember> _propertyRule;

        public RuleFact(Rule<DataType> typeRule, Rule<Set> setRule, Rule<DataMember> propertyRule)
        {
            _typeRule = typeRule;
            _setRule = setRule;
            _propertyRule = propertyRule;
        }

        public override bool Start(Symbol symbol)
        {
            return symbol == Symbol.Fact;
        }

        public override Fact Match(TokenStream tokenStream)
        {
            Token factToken = tokenStream.Expect(Symbol.Fact, "Declare a fact.");
            Token factNameToken = tokenStream.Expect(Symbol.Identifier, "Provide a name for the fact.");

            Fact fact = new Fact(factNameToken.Value, factNameToken.LineNumber);

            Token openBracketToken = tokenStream.Expect(Symbol.OpenBracket, "Declare members of a fact within brackets.");

            while (true)
            {
                if (_typeRule.Start(tokenStream.Lookahead.Symbol))
                {
                    DataType type = _typeRule.Match(tokenStream);
                    Token nameToken = tokenStream.Expect(Symbol.Identifier, "Provide a name for the field or query.");
                    if (tokenStream.Lookahead.Symbol == Symbol.OpenBracket)
                    {
                        tokenStream.Consume();

                        DataTypeFact factType = type as DataTypeFact;
                        if (factType == null)
                            throw new FactualException("A query must return a fact type, not a native type.", type.LineNumber);
                        if (factType.Cardinality != Cardinality.Many)
                            throw new FactualException("A query must return multiple results.", factType.LineNumber);

                        Query query = new Query(nameToken.Value, factType.FactName, factType.LineNumber);

                        while (_setRule.Start(tokenStream.Lookahead.Symbol))
                            query.AddSet(_setRule.Match(tokenStream));
                        fact.AddQuery(query);

                        tokenStream.Expect(Symbol.CloseBracket, "A query must contain sets.");
                    }
                    else
                    {
                        tokenStream.Expect(Symbol.Semicolon, "Terminate a field with a semicolon.");
                        fact.AddField(new DataMember(type.LineNumber, nameToken.Value, type));
                    }
                }
                else if (_propertyRule.Start(tokenStream.Lookahead.Symbol))
                    fact.AddProperty(_propertyRule.Match(tokenStream));
                else
                    break;
            }

            Token closeBracketToken = tokenStream.Expect(Symbol.CloseBracket, "A member must be a field, property, query, or predicate.");

            return fact;
        }
    }
    public class FactualParser : Parser<Namespace>
    {
        public FactualParser(System.IO.TextReader input)
            : base(
                Lexer(input),
                Rule(),
                "Add a 'namespace' declaration.",
                "Declare a fact."
            )
        {
        }

        private static Lexer Lexer(System.IO.TextReader input)
        {
            return new Lexer(input)
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
                .AddSymbol("=", Symbol.Equal);
        }

        private static Rule<Namespace> Rule()
        {
            var dottedIdentifier = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                identifier => new StringBuilder().Append(identifier.Value),
                (stringBuilder, identifier) => stringBuilder.Append(".").Append(identifier.Value));
            var relativePath = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                segment => new PathRelative().AddSegment(segment.Value),
                (path, segment) => path.AddSegment(segment.Value));
            var namespaceRule = Sequence(
                Terminal(Symbol.Namespace),
                dottedIdentifier, "Provide a dotted identifier for the namespace.",
                Terminal(Symbol.Semicolon), "Terminate the namespace declaration with a semicolon.",
                (namespaceToken, identifier, ignored) => new Namespace(identifier.ToString(), namespaceToken.LineNumber));
            var pathRule =
                Translate(relativePath, path => (Path)path) |
                Translate(Terminal(Symbol.This), t => (Path)new PathAbsolute());
            var cardinalityRule = Optional(
                Translate(Terminal(Symbol.Question), t => Cardinality.Optional) |
                Translate(Terminal(Symbol.Asterisk), t => Cardinality.Many),
                Cardinality.One);
            var nativeTypeNameRule =
                Translate(Terminal(Symbol.String), t => new NativeTypeAtLine(NativeType.String, t.LineNumber)) |
                Translate(Terminal(Symbol.Int),    t => new NativeTypeAtLine(NativeType.Int,    t.LineNumber)) |
                Translate(Terminal(Symbol.Float),  t => new NativeTypeAtLine(NativeType.Float,  t.LineNumber)) |
                Translate(Terminal(Symbol.Char),   t => new NativeTypeAtLine(NativeType.Char,   t.LineNumber)) |
                Translate(Terminal(Symbol.Date),   t => new NativeTypeAtLine(NativeType.Date,   t.LineNumber)) |
                Translate(Terminal(Symbol.Time),   t => new NativeTypeAtLine(NativeType.Time,   t.LineNumber));
            var nativeTypeRule = Sequence(
                nativeTypeNameRule,
                cardinalityRule, "Defect.",
                (nativeType, cardinality) => new DataTypeNative(nativeType.NativeType, cardinality, nativeType.LineNumber));
            var factTypeRule = Sequence(
                Terminal(Symbol.Identifier),
                cardinalityRule, "Defect.",
                (identifier, cardinality) => new DataTypeFact(identifier.Value, cardinality, identifier.LineNumber));
            var typeRule =
                Translate(nativeTypeRule, t => (DataType)t) |
                Translate(factTypeRule, t => (DataType)t);
            var propertyRule = Sequence(
                Terminal(Symbol.Property),
                typeRule, "Declare a type for the property.",
                Terminal(Symbol.Identifier), "Provide a name for the property.",
                Terminal(Symbol.Semicolon), "Terminate a property with a semicolon.",
                (propertyToken, dataType, propertyNameToken, semicolonToken) => new DataMember(propertyToken.LineNumber, propertyNameToken.Value, dataType));
            var setRule = Sequence(
                Terminal(Symbol.Identifier),
                Terminal(Symbol.Identifier), "Provide a name for the set.",
                Terminal(Symbol.Colon), "Use a colon to separate the set name from its definition.",
                pathRule, "Declare a relative path or \"this\".",
                Terminal(Symbol.Equal), "Separate paths with an equal sign.",
                pathRule, "Declare a relative path or \"this\".",
                (factNameToken, setNameToken, colonToken, leftPath, equalToken, rightPath) => new Set(setNameToken.Value, factNameToken.Value, leftPath, rightPath, factNameToken.LineNumber));
            var factRule = new RuleFact(typeRule, setRule, propertyRule);
            var rule = Many(namespaceRule, factRule, (n, f) => n.AddFact(f));
            return rule;
        }
    }
    public abstract class Parser<T>
    {
        private TokenStream _tokenStream;
        private Rule<T> _rule;
        private string _errorStart;
        private string _errorEnd;

        protected Parser(Lexer lexer, Rule<T> rule, string errorStart, string errorEnd)
        {
            _tokenStream = new TokenStream(lexer);
            _rule = rule;
            _errorStart = errorStart;
            _errorEnd = errorEnd;
        }

        protected static Rule<Token> Terminal(Symbol expectedSymbol)
        {
            return new RuleTerminal(expectedSymbol);
        }

        protected static Rule<T> Translate<TFrom, T>(Rule<TFrom> ruleFrom, Func<TFrom, T> translation)
        {
            return new RuleTranslate<TFrom, T>(ruleFrom, translation);
        }

        protected static Rule<T> Optional<T>(Rule<T> rule, T defaultValue)
        {
            return new RuleOptional<T>(rule, defaultValue);
        }

        protected static Rule<T> Separated<T, TItem>(Rule<TItem> itemRule, Symbol separator, Func<TItem, T> begin, Func<T, TItem, T> append)
        {
            return new RuleSeparated<T, TItem>(itemRule, separator, begin, append);
        }

        protected static Rule<T> Many<TItem, T>(Rule<T> headerRule, Rule<TItem> itemRule, Func<T, TItem, T> append)
        {
            return new RuleMany<TItem, T>(headerRule, itemRule, append);
        }

        protected static Rule<T> Sequence<T1, T2, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, RuleSequence2<T1, T2, T>.Function reduce)
        {
            return new RuleSequence2<T1, T2, T>(rule1, rule2, error2, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, RuleSequence3<T1, T2, T3, T>.Function reduce)
        {
            return new RuleSequence3<T1, T2, T3, T>(rule1, rule2, error2, rule3, error3, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, RuleSequence4<T1, T2, T3, T4, T>.Function reduce)
        {
            return new RuleSequence4<T1, T2, T3, T4, T>(rule1, rule2, error2, rule3, error3, rule4, error4, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T5, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Rule<T5> rule5, string error5, RuleSequence5<T1, T2, T3, T4, T5, T>.Function reduce)
        {
            return new RuleSequence5<T1, T2, T3, T4, T5, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, reduce);
        }

        protected static Rule<T> Sequence<T1, T2, T3, T4, T5, T6, T>(Rule<T1> rule1, Rule<T2> rule2, string error2, Rule<T3> rule3, string error3, Rule<T4> rule4, string error4, Rule<T5> rule5, string error5, Rule<T6> rule6, string error6, RuleSequence6<T1, T2, T3, T4, T5, T6, T>.Function reduce)
        {
            return new RuleSequence6<T1, T2, T3, T4, T5, T6, T>(rule1, rule2, error2, rule3, error3, rule4, error4, rule5, error5, rule6, error6, reduce);
        }

        public T Parse()
        {
            _tokenStream.Consume();

            if (!_rule.Start(_tokenStream.Lookahead.Symbol))
                throw new FactualException(_errorStart, _tokenStream.Lookahead.LineNumber);

            T namespaceNode = _rule.Match(_tokenStream);

            if (_tokenStream.Lookahead.Symbol != Symbol.EndOfFile)
                throw new FactualException(_errorEnd, _tokenStream.Lookahead.LineNumber);

            return namespaceNode;
        }
    }
}
