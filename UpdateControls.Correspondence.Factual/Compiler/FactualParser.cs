using System;
using UpdateControls.Correspondence.Factual.AST;
using System.Text;
using QEDCode.LLOne;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public class FactualParser : Parser<Symbol, Namespace>
    {
        private static readonly FactSection EmptySection = new FactSection();

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
                .AddSymbol("strength", Symbol.Strength)
                .AddSymbol("fact", Symbol.Fact)
                .AddSymbol("property", Symbol.Property)
                .AddSymbol("this", Symbol.This)
				.AddSymbol("string", Symbol.String)
                .AddSymbol("byte", Symbol.Byte)
				.AddSymbol("int", Symbol.Int)
				.AddSymbol("long", Symbol.Long)
				.AddSymbol("float", Symbol.Float)
				.AddSymbol("double", Symbol.Double)
				.AddSymbol("decimal", Symbol.Decimal)
				.AddSymbol("char", Symbol.Char)
                .AddSymbol("date", Symbol.Date)
                .AddSymbol("time", Symbol.Time)
                .AddSymbol("bool", Symbol.Bool)
                .AddSymbol("not", Symbol.Not)
                .AddSymbol("exists", Symbol.Exists)
                .AddSymbol("where", Symbol.Where)
                .AddSymbol("and", Symbol.And)
                .AddSymbol("unique", Symbol.Unique)
				.AddSymbol("publish", Symbol.Publish)
                .AddSymbol("principal", Symbol.Principal)
                .AddSymbol("from", Symbol.From)
                .AddSymbol("to", Symbol.To)
                .AddSymbol("key", Symbol.Key)
                .AddSymbol("query", Symbol.Query)
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
            // strength_declaration_opt -> ("strength" identifier ";")?
            // namespace_declaration -> "namespace" dotted_identifier ";" strength_declaration_opt
            var dottedIdentifier = Separated(
                Terminal(Symbol.Identifier),
                Symbol.Dot,
                identifier => new StringBuilder().Append(identifier.Value),
                (stringBuilder, identifier) => stringBuilder.Append(".").Append(identifier.Value));
            var strengthDeclaration = Sequence(
                Terminal(Symbol.Strength),
                Terminal(Symbol.Identifier), "Provide a strength identifier.",
                Terminal(Symbol.Semicolon), "Terminate the strength declaration with a semicolon.",
                (strengthToken, identifier, ignored) => identifier.Value);
            var namespaceRule = Sequence(
                Terminal(Symbol.Namespace),
                dottedIdentifier, "Provide a dotted identifier for the namespace.",
                Terminal(Symbol.Semicolon), "Terminate the namespace declaration with a semicolon.",
                Optional(strengthDeclaration, string.Empty), "Defect.",
                (namespaceToken, identifier, ignored, strengthIdentifier) => new Namespace(identifier.ToString(), namespaceToken.LineNumber, strengthIdentifier));

            // path -> (identifier | "this") ("." identifier)*
            var pathRule =
                Many(
                    Reduce(Terminal(Symbol.Identifier), t => new Path(false, t.Value)) |
                    Reduce(Terminal(Symbol.This), t => new Path(true, null)),
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
                Optional(Reduce(Terminal(Symbol.Publish), t => true), false),
                Terminal(Symbol.Identifier), "Defect.",
                cardinalityRule, "Defect.",
                (isPivot, identifier, cardinality) => new DataTypeFact(identifier.Value, cardinality, isPivot, identifier.LineNumber));
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

            // clause -> "not"? identifier "." identifier
            // condition -> "where" clause ("and" clause)*
            var clauseRule = Sequence(
                Optional(Reduce(Terminal(Symbol.Not), t => ConditionModifier.Negative), ConditionModifier.Positive),
                Terminal(Symbol.Identifier), "Provide the name of a set.",
                Terminal(Symbol.Dot), "Use a dot to reference a predicate.",
                Terminal(Symbol.Identifier), "Provide the name of a predicate.",
                (not, set, dot, predicate) => new Clause(set.LineNumber, not, set.Value, predicate.Value));
            var conditionRule = Many(
                Sequence(
                    Terminal(Symbol.Where),
                    clauseRule, "Give a predicate to use as a condition.",
                    (whereToken, clause) => new Condition().AddClause(clause)),
                Sequence(
                    Terminal(Symbol.And),
                    clauseRule, "Provide another clause.",
                    (and, clause) => clause),
                (condition, clause) => condition.AddClause(clause));

            // query_tail -> "{" set* "}"
            // set -> identifier identifier ":" path "=" path condition?
            var setRule = Sequence(
                Terminal(Symbol.Identifier),
                Terminal(Symbol.Identifier), "Provide a name for the set.",
                Terminal(Symbol.Colon), "Use a colon to separate the set name from its definition.",
                pathRule, "Declare a relative path or \"this\".",
                Terminal(Symbol.Equal), "Separate paths with an equal sign.",
                pathRule, "Declare a relative path or \"this\".",
                Optional(conditionRule, new Condition()), "Defect.",
                (factNameToken, setNameToken, colonToken, leftPath, equalToken, rightPath, condition) => new Set(setNameToken.Value, factNameToken.Value, leftPath, rightPath, condition, factNameToken.LineNumber));
            var queryTailRule = Sequence(
                Many(
                    Reduce(Terminal(Symbol.OpenBracket), t => new QueryTail()),
                    setRule,
                    (queryTail, set) => queryTail.AddSet(set)),
                Terminal(Symbol.CloseBracket), "A query must contain sets.",
                (queryTail, closeBracket) => QueryGenerator(queryTail));

            // predicate -> "bool" identifier "{" "not"? "exists" set* "}"
            var predicateRule = Sequence(
                Many(
                    Sequence(
                        Terminal(Symbol.Bool),
                        Terminal(Symbol.Identifier), "Provide a name for the predicate.",
                        Terminal(Symbol.OpenBracket), "Declare sets of a predicate inside of brackets.",
                        Optional(Reduce(Terminal(Symbol.Not), t => ConditionModifier.Negative), ConditionModifier.Positive), "Defect",
                        Terminal(Symbol.Exists), "Use the keyword \"exists\" to indicate the set of facts to test.",
                        (boolToken, nameToken, openBracket, existence, existsToken) => new Predicate(nameToken.Value, existence, boolToken.LineNumber)),
                    setRule,
                    (predicate, set) => predicate.AddSet(set)),
                Terminal(Symbol.CloseBracket), "A predicate must contain sets.",
                (predicate, closeBracket) => (FactMember)predicate);

            // field_tail -> ";"
            // field_or_query -> type identifier (field_tail | query_tail)
            var fieldTailRule = Reduce(Terminal(Symbol.Semicolon), t => FieldGenerator());
            var fieldOrQueryRule = Sequence(
                typeRule,
                Terminal(Symbol.Identifier), "Provide a name for the field or query.",
                fieldTailRule | queryTailRule, "Declare a field or a query.",
                (type, nameToken, generator) => generator(type, nameToken));

            // secure_field -> ("from" | "to") type identifier ";"
            var secureFieldRule = Sequence(
                (Terminal(Symbol.From) | Terminal(Symbol.To)),
                typeRule, "Declare a field after \"from\" or \"to\".",
                Terminal(Symbol.Identifier), "Provide a name for the field.",
                Terminal(Symbol.Semicolon), "A field is followed by a semicolon.",
                (modifierToken, typeToken, nameToken, semicolon) => CreateSecureField(modifierToken, typeToken, nameToken.Value)
            );

            // key_member -> field | secure_field
            // key_section -> "key" ":" key_member*
            var keyMemberRule = fieldOrQueryRule | secureFieldRule;
            var keySectionRule = Many(
                Sequence(
                    Terminal(Symbol.Key),
                    Terminal(Symbol.Colon), "Missing \":\" after \"key\".",
                    (key, colon) => new FactSection()),
                keyMemberRule, (keySection, keyMember) => keySection.AddMember(keyMember));

            // query_member -> query | predicate
            // query_section -> "query" ":" query_member*
            var queryMemberRule = fieldOrQueryRule | predicateRule;
            var querySectionRule = Many(
                Sequence(
                    Terminal(Symbol.Query),
                    Terminal(Symbol.Colon), "Missing \":\" after \"query\".",
                    (key, colon) => new FactSection()),
                queryMemberRule, (querySection, queryMember) => querySection.AddMember(queryMember));

            // fact -> "fact" identifier "{" key_section? query_section? modifier* member* "}"
            // member -> field_or_query | secure_feld | property | predicate
            var factMemberRule = propertyRule;
            var factHeader = Sequence(
                Terminal(Symbol.Fact),
                Terminal(Symbol.Identifier), "Provide a name for the fact.",
                Terminal(Symbol.OpenBracket), "Declare members of a fact within brackets.",
                (fact, identifier, openBracket) => new Fact(identifier.Value, fact.LineNumber));
            var principalModifierRule = Sequence(
                Terminal(Symbol.Principal),
                Terminal(Symbol.Semicolon), "The principal modifier is followed by a semicolon.",
                (modifier, semicolon) => modifier);
            var uniqueModifierRule = Sequence(
                Terminal(Symbol.Unique),
                Terminal(Symbol.Semicolon), "The unique modifier is followed by a semicolon.",
                (modifier, semicolon) => modifier);
            var modifierRule = uniqueModifierRule | principalModifierRule;
            var sectionedFactHeader = Sequence(
                factHeader,
                Optional(keySectionRule, EmptySection), "Defect.",
                Optional(querySectionRule, EmptySection), "Defect.",
                (fact, keySection, querySection) => querySection.AddTo(keySection.AddTo(fact)));
            var modifiedFactHeader = Many(
                sectionedFactHeader, modifierRule, (fact, modifier) => ModifyFact(fact, modifier.Symbol));
            var factRule = Sequence(
                Many(modifiedFactHeader, factMemberRule, (fact, member) => fact.AddMember(member)),
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

        private static Fact ModifyFact(Fact fact, Symbol modifier)
        {
            if (modifier == Symbol.Unique)
                fact.Unique = true;
            else if (modifier == Symbol.Principal)
                fact.Principal = true;
            return fact;
        }

        private static FactMember CreateSecureField(Token<Symbol> modifierToken, DataType dataType, string name)
        {
            Field field = new Field(modifierToken.LineNumber, name, dataType);
            if (modifierToken.Symbol == Symbol.From)
                field.SecurityModifier = FieldSecurityModifier.From;
            else if (modifierToken.Symbol == Symbol.To)
                field.SecurityModifier = FieldSecurityModifier.To;
            return field;
        }
    }
}
