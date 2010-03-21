using System;
using UpdateControls.Correspondence.Factual.AST;
using System.Text;
using QEDCode.LLOne;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class FactualParser : Parser<Symbol, Namespace>
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

        private static Lexer<Symbol> Lexer(System.IO.TextReader input)
        {
            return new Lexer<Symbol>(input, Symbol.Identifier, Symbol.EndOfFile)
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

        private static Rule<Symbol, Namespace> Rule()
        {
            // dotted_identifier -> identifier ("." identifier)*
            // namespace_declaration -> "namespace" dotted_identifier ";"
            var dottedIdentifier = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                identifier => new StringBuilder().Append(identifier.Value),
                (stringBuilder, identifier) => stringBuilder.Append(".").Append(identifier.Value));
            var namespaceRule = Sequence(
                Terminal(Symbol.Namespace),
                dottedIdentifier, "Provide a dotted identifier for the namespace.",
                Terminal(Symbol.Semicolon), "Terminate the namespace declaration with a semicolon.",
                (namespaceToken, identifier, ignored) => new Namespace(identifier.ToString(), namespaceToken.LineNumber));

            // path -> (identifier | "this") ("." identifier)*
            var pathRule =
                Many(
                    Reduce(Terminal(Symbol.Identifier), t => new Path(false, t.Value)) |
                    Reduce(Terminal(Symbol.This), t => new Path(true, "this")),
                    Sequence(
                        Terminal(Symbol.Dot),
                        Terminal(Symbol.Identifier),
                        "Provide a predecessor after the dot.",
                        (dot, identifier) => identifier
                    ),
                    (path, segment) => path.AddSegment(segment.Value)
                );

            // type -> (native_type | identifier) ("?" | "*")?
            // native_type -> "int" | "float" | "char" | "string" | "date" | "time"
            var nativeTypeNameRule =
                Reduce(Terminal(Symbol.String), t => new NativeTypeAtLine(NativeType.String, t.LineNumber)) |
                Reduce(Terminal(Symbol.Int), t => new NativeTypeAtLine(NativeType.Int, t.LineNumber)) |
                Reduce(Terminal(Symbol.Float), t => new NativeTypeAtLine(NativeType.Float, t.LineNumber)) |
                Reduce(Terminal(Symbol.Char), t => new NativeTypeAtLine(NativeType.Char, t.LineNumber)) |
                Reduce(Terminal(Symbol.Date), t => new NativeTypeAtLine(NativeType.Date, t.LineNumber)) |
                Reduce(Terminal(Symbol.Time), t => new NativeTypeAtLine(NativeType.Time, t.LineNumber));
            var cardinalityRule = Optional(
                Reduce(Terminal(Symbol.Question), t => Cardinality.Optional) |
                Reduce(Terminal(Symbol.Asterisk), t => Cardinality.Many),
                Cardinality.One);
            var nativeTypeRule = Sequence(
                nativeTypeNameRule,
                cardinalityRule, "Defect.",
                (nativeType, cardinality) => new DataTypeNative(nativeType.NativeType, cardinality, nativeType.LineNumber));
            var factTypeRule = Sequence(
                Terminal(Symbol.Identifier),
                cardinalityRule, "Defect.",
                (identifier, cardinality) => new DataTypeFact(identifier.Value, cardinality, identifier.LineNumber));
            var typeRule =
                Reduce(nativeTypeRule, t => (DataType)t) |
                Reduce(factTypeRule, t => (DataType)t);

            // property -> "property" type identifier ";"
            var propertyRule = Sequence(
                Terminal(Symbol.Property),
                typeRule, "Declare a type for the property.",
                Terminal(Symbol.Identifier), "Provide a name for the property.",
                Terminal(Symbol.Semicolon), "Terminate a property with a semicolon.",
                (propertyToken, dataType, propertyNameToken, semicolonToken) => (FactMember)new Property(propertyToken.LineNumber, propertyNameToken.Value, dataType));

            // query_tail -> "{" set* "}"
            // set -> identifier identifier ":" path "=" path condition?
            var setRule = Sequence(
                Terminal(Symbol.Identifier),
                Terminal(Symbol.Identifier), "Provide a name for the set.",
                Terminal(Symbol.Colon), "Use a colon to separate the set name from its definition.",
                pathRule, "Declare a relative path or \"this\".",
                Terminal(Symbol.Equal), "Separate paths with an equal sign.",
                pathRule, "Declare a relative path or \"this\".",
                (factNameToken, setNameToken, colonToken, leftPath, equalToken, rightPath) => new Set(setNameToken.Value, factNameToken.Value, leftPath, rightPath, factNameToken.LineNumber));
            var queryTailRule = Sequence(
                Many(
                    Reduce(Terminal(Symbol.OpenBracket), t => new QueryTail()),
                    setRule,
                    (queryTail, set) => queryTail.AddSet(set)),
                Terminal(Symbol.CloseBracket), "A query must contain sets.",
                (queryTail, closeBracket) => QueryGenerator(queryTail));

            // field_tail -> ";"
            // field_or_query -> type identifier (field_tail | query_tail)
            var fieldTailRule = Reduce(Terminal(Symbol.Semicolon), t => FieldGenerator());
            var fieldOrQueryRule = Sequence(
                typeRule,
                Terminal(Symbol.Identifier), "Provide a name for the field or query.",
                fieldTailRule | queryTailRule, "Declare a field or a query.",
                (type, nameToken, generator) => generator(type, nameToken));

            // fact -> "fact" identifier "{" member* "}"
            // member -> field_or_query | property
            var factMemberRule = fieldOrQueryRule | propertyRule;
            var factHeader = Sequence(
                Terminal(Symbol.Fact),
                Terminal(Symbol.Identifier), "Provide a name for the fact.",
                Terminal(Symbol.OpenBracket), "Declare members of a fact within brackets.",
                (fact, identifier, openBracket) => new Fact(identifier.Value, fact.LineNumber));
            var factRule = Sequence(
                Many(factHeader, factMemberRule, (fact, member) => fact.AddMember(member)),
                Terminal(Symbol.CloseBracket), "A member must be a field, property, query, or predicate.",
                (fact, closeBracket) => fact);

            // factual_file -> namespace_declaration fact*
            var rule = Many(namespaceRule, factRule, (namespaceRoot, fact) => namespaceRoot.AddFact(fact));
            return rule;
        }

        private static Func<DataType, Token<Symbol>, FactMember> QueryGenerator(QueryTail queryTail)
        {
            return (type, nameToken) =>
            {
                DataTypeFact factType = type as DataTypeFact;
                if (factType == null)
                    throw new ParserException("A query must return a fact type, not a native type.", type.LineNumber);
                if (factType.Cardinality != Cardinality.Many)
                    throw new ParserException("A query must return multiple results.", factType.LineNumber);

                return new Query(nameToken.Value, factType.FactName, queryTail, factType.LineNumber);
            };
        }

        private static Func<DataType, Token<Symbol>, FactMember> FieldGenerator()
        {
            return (type, nameToken) => new Field(type.LineNumber, nameToken.Value, type);
        }
    }
}
