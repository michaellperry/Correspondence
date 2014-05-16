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
                "Add a 'namespace' declaration."
            )
        {
        }

        public static Lexer<Symbol> Lexer(System.IO.TextReader input)
        {
            return new Lexer<Symbol>(input, Symbol.Identifier, Symbol.QuotedString, Symbol.Number, Symbol.EndOfFile)
                .AddSymbol("namespace", Symbol.Namespace)
                .AddSymbol("strength", Symbol.Strength)
                .AddSymbol("fact", Symbol.Fact)
                .AddSymbol("this", Symbol.This)
				.AddSymbol("string", Symbol.String)
                .AddSymbol("byte", Symbol.Byte)
                .AddSymbol("binary", Symbol.Binary)
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
                .AddSymbol("mutable", Symbol.Mutable)
                .AddSymbol("exit", Symbol.Exit)
                .AddSymbol("connect", Symbol.Connect)
                .AddSymbol("subscribe", Symbol.Subscribe)
                .AddSymbol("purge", Symbol.Purge)
                .AddSymbol("lock", Symbol.Lock)
                .AddSymbol(".", Symbol.Dot)
                .AddSymbol(";", Symbol.Semicolon)
                .AddSymbol("{", Symbol.OpenBracket)
                .AddSymbol("}", Symbol.CloseBracket)
                .AddSymbol("?", Symbol.Question)
                .AddSymbol("*", Symbol.Asterisk)
                .AddSymbol(":", Symbol.Colon)
                .AddSymbol("=", Symbol.Equal)
                .AddSymbol(",", Symbol.Comma)
                .AddSymbol("(", Symbol.OpenParentheses)
                .AddSymbol(")", Symbol.CloseParentheses);
        }

        public static Rule<Symbol, Namespace> GetNamespaceRule()
        {
            // dotted_identifier -> identifier ("." identifier)*
            // header_declaration_list -> (identifier identifier ";")*
            // strength_declaration_opt -> ("strength" identifier ";")?
            // namespace_declaration -> "namespace" dotted_identifier ";" header_declaration_list strength_declaration_opt
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
            var headerDeclarationList = Many(
                new List<Header>(),
                Sequence(
                    Terminal(Symbol.Identifier),
                    Terminal(Symbol.Identifier), "Provide a parameter to the header.",
                    Terminal(Symbol.Semicolon), "Terminate a header with a semicolon.",
                    (name, parameter, semi) => new Header(name.Value, parameter.Value)),
                (headers, header) => { headers.Add(header); return headers; });
            var namespaceRule = Sequence(
                Terminal(Symbol.Namespace),
                dottedIdentifier, "Provide a dotted identifier for the namespace.",
                Terminal(Symbol.Semicolon), "Terminate the namespace declaration with a semicolon.",
                headerDeclarationList, "Defect.",
                Optional(strengthDeclaration, string.Empty), "Defect.",
                (namespaceToken, identifier, ignored, headers, strengthIdentifier) => new Namespace(identifier.ToString(), namespaceToken.LineNumber, headers, strengthIdentifier));
            return namespaceRule;
        }

        public static Rule<Symbol, Fact> GetFactRule()
        {
            // type -> (native_type | identifier) ("?" | "*")?
            // native_type -> "int" | "float" | "char" | "string" | "date" | "time" | "byte"
            var nativeTypeNameRule =
                Reduce(Terminal(Symbol.String), t => new NativeTypeAtLine(NativeType.String, t.LineNumber)) |
                Reduce(Terminal(Symbol.Int), t => new NativeTypeAtLine(NativeType.Int, t.LineNumber)) |
                Reduce(Terminal(Symbol.Float), t => new NativeTypeAtLine(NativeType.Float, t.LineNumber)) |
                Reduce(Terminal(Symbol.Char), t => new NativeTypeAtLine(NativeType.Char, t.LineNumber)) |
                Reduce(Terminal(Symbol.Date), t => new NativeTypeAtLine(NativeType.Date, t.LineNumber)) |
                Reduce(Terminal(Symbol.Time), t => new NativeTypeAtLine(NativeType.Time, t.LineNumber)) |
                Reduce(Terminal(Symbol.Byte), t => new NativeTypeAtLine(NativeType.Byte, t.LineNumber)) |
                Reduce(Terminal(Symbol.Binary), t => new NativeTypeAtLine(NativeType.Binary, t.LineNumber));
            var cardinalityRule = Optional(
                Reduce(Terminal(Symbol.Question), t => Cardinality.Optional) |
                Reduce(Terminal(Symbol.Asterisk), t => Cardinality.Many),
                Cardinality.One);
            var nativeTypeRule = Sequence(
                nativeTypeNameRule,
                cardinalityRule, "Defect.",
                (nativeType, cardinality) => new DataTypeNative(nativeType.NativeType, cardinality, nativeType.LineNumber));
            var publishOptRule = Optional(Reduce(Terminal(Symbol.Publish), t => true), false);
            var factTypeRule = Sequence(
                Terminal(Symbol.Identifier),
                cardinalityRule, "Defect.",
                (identifier, cardinality) => new DataTypeFact(identifier.Value, cardinality, identifier.LineNumber));
            var typeRule =
                Reduce(nativeTypeRule, t => (DataType)t) |
                Reduce(factTypeRule, t => (DataType)t);

            var setRule = GetSetRule();

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

            // collaborator -> ("from" | "to") local_path ";"
            // local_path -> identifier ("." identifier)*
            var localPathRule = Many(
                Reduce(
                    Terminal(Symbol.Identifier),
                    (id) => new Path(true, null).AddSegment(id.Value)),
                Sequence(
                    Terminal(Symbol.Dot),
                    Terminal(Symbol.Identifier), "Provide a predecessor after the dot.",
                    (dot, segment) => segment.Value),
                (path, segment) => path.AddSegment(segment));
            var collaboratorRule = Sequence(
                Terminal(Symbol.To),
                localPathRule, "Specify a path for the collaborator.",
                Terminal(Symbol.Semicolon), "Terminate the path with a semicolon.",
                (modifier, path, semi) => (Func<FactSection, FactSection>)(keySection =>
                    keySection.SetToPath(path)));

            // field -> "publish"? type identifier publish_condition? ";"
            // publish_clause -> "not"? "this" "." identifier
            // publish_condition -> "where" clause ("and" clause)*
            var publishClauseRule = Sequence(
                Optional(Reduce(Terminal(Symbol.Not), t => ConditionModifier.Negative), ConditionModifier.Positive),
                Terminal(Symbol.This), "Publish conditions must be based on this.",
                Terminal(Symbol.Dot), "Use a dot to reference a predicate.",
                Terminal(Symbol.Identifier), "Provide the name of a predicate.",
                (not, set, dot, predicate) => new Clause(set.LineNumber, not, set.Value, predicate.Value));
            var publishConditionRule = Many(
                Sequence(
                    Terminal(Symbol.Where),
                    publishClauseRule, "Give a predicate to use as a condition.",
                    (whereToken, clause) => new Condition().AddClause(clause)),
                Sequence(
                    Terminal(Symbol.And),
                    publishClauseRule, "Provide another clause.",
                    (and, clause) => clause),
                (condition, clause) => condition.AddClause(clause));
            var simpleFieldRule = Sequence(
                typeRule,
                Terminal(Symbol.Identifier), "Provide a name for the field.",
                Terminal(Symbol.Semicolon), "Missing semicolon after field.",
                (type, nameToken, semicolon) => CreateField(type, nameToken.Value, false, null));
            var publishFieldRule = Sequence(
                Terminal(Symbol.Publish),
                typeRule, "Provide a field type.",
                Terminal(Symbol.Identifier), "Provide a name for the field.",
                Optional(publishConditionRule, new Condition()), "Defect.",
                Terminal(Symbol.Semicolon), "Missing semicolon after field.",
                (publish, type, nameToken, condition, semicolon) => CreateField(type, nameToken.Value, true, condition));
            var fieldRule = simpleFieldRule | publishFieldRule;

            // query -> type identifier "{" set* "}"
            var queryRule = Sequence(
                Many(
                    Sequence(
                        typeRule,
                        Terminal(Symbol.Identifier), "Provide a name for the query.",
                        Terminal(Symbol.OpenBracket), "Provide query sets in brackets.",
                        (type, name, openBracket) => CreateQuery(type, name)),
                    setRule, (query, set) => query.AddSet(set)),
                Terminal(Symbol.CloseBracket), "A query must contain sets.",
                (query, closeBracket) => (FactMember)query);

            // purge_section -> "purge" "where" clause ("and" clause)* ";"
            var purgeSectionRule = Sequence(
                Many(
                    Sequence(
                        Terminal(Symbol.Purge),
                        Terminal(Symbol.Where), "Purge must be followed by where.",
                        publishClauseRule, "Give a predicate to use as a condition.",
                        (purgeToken, whereToken, clause) => new Condition().AddClause(clause)),
                    Sequence(
                        Terminal(Symbol.And),
                        publishClauseRule, "Provide another clause.",
                        (and, clause) => clause),
                    (condition, clause) => condition.AddClause(clause)),
                Terminal(Symbol.Semicolon), "Terminate a purge condition with a semicolon.",
                (condition, semicolonToken) => condition);

            // key_member -> modifier | field | collaborator
            // key_section -> "key" ":" key_member*
            var principalModifierRule = Sequence(
                Terminal(Symbol.Principal),
                Terminal(Symbol.Semicolon), "The principal modifier is followed by a semicolon.",
                (modifier, semicolon) => (Func<FactSection, FactSection>)(keySection => keySection.SetPrincipal()));
            var uniqueModifierRule = Sequence(
                Terminal(Symbol.Unique),
                Terminal(Symbol.Semicolon), "The unique modifier is followed by a semicolon.",
                (modifier, semicolon) => (Func<FactSection, FactSection>)(keySection => keySection.SetUnique()));
            var lockModifierRule = Sequence(
                Terminal(Symbol.Lock),
                Terminal(Symbol.Semicolon), "The lock modifier is followed by a semicolon.",
                (modifier, semicolon) => (Func<FactSection, FactSection>)(keySection => keySection.SetLock()));
            var modifierRule = uniqueModifierRule | lockModifierRule | principalModifierRule;
            var keyMemberRule = Reduce(fieldRule, keyMember => (Func<FactSection, FactSection>)(keySection => keySection.AddMember(keyMember)));
            var keySectionRule = Many(
                Sequence(
                    Terminal(Symbol.Key),
                    Terminal(Symbol.Colon), "Missing \":\" after \"key\".",
                    (key, colon) => new FactSection()),
                keyMemberRule | modifierRule | collaboratorRule, (keySection, keyMember) => keyMember(keySection));

            // mutable_member -> "publish"? type identifier ";"
            // mutable_section -> "mutable" ":" mutable_member*
            var mutableMemberRule = Sequence(
                publishOptRule,
                typeRule, "Provide a field type.",
                Terminal(Symbol.Identifier), "Provide a name for the field.",
                Error(Symbol.Where, "A mutable field may not have a publish condition."), "Defect.",
                Terminal(Symbol.Semicolon), "Terminate a field with a semicolon.",
                (publish, dataType, propertyNameToken, error, semicolonToken) => (FactMember)new Property(dataType.LineNumber, propertyNameToken.Value, dataType, publish));
            var mutableSectionRule = Many(
                Sequence(
                    Terminal(Symbol.Mutable),
                    Terminal(Symbol.Colon), "Missing \":\" after \"mutable\".",
                    (key, colon) => new FactSection()),
                mutableMemberRule, (mutableSection, mutableMember) => mutableSection.AddMember(mutableMember));

            // query_member -> query | predicate
            // query_section -> "query" ":" query_member*
            var queryMemberRule = queryRule | predicateRule;
            var querySectionRule = Many(
                Sequence(
                    Terminal(Symbol.Query),
                    Terminal(Symbol.Colon), "Missing \":\" after \"query\".",
                    (key, colon) => new FactSection()),
                queryMemberRule, (querySection, queryMember) => querySection.AddMember(queryMember));

            // fact -> "fact" identifier "{" purge_section? key_section? mutable_section? query_section? "}"
            var factHeader = Sequence(
                Terminal(Symbol.Fact),
                Terminal(Symbol.Identifier), "Provide a name for the fact.",
                Terminal(Symbol.OpenBracket), "Declare members of a fact within brackets.",
                (fact, identifier, openBracket) => new Fact(identifier.Value, fact.LineNumber));
            var factRule = Sequence(
                factHeader,
                Optional(purgeSectionRule, (Condition)null), "Defect.",
                Optional(keySectionRule, EmptySection), "Defect.",
                Optional(mutableSectionRule, EmptySection), "Defect.",
                Optional(querySectionRule, EmptySection), "Defect.",
                Terminal(Symbol.CloseBracket), "Specify a \"purge\", \"key\", \"mutable\", or \"query\" section.",
                (fact, purgeCondition, keySection, mutableSection, querySection, closeBracket) =>
                    {
                        keySection.AddTo(fact);
                        mutableSection.AddTo(fact);
                        querySection.AddTo(fact);
                        fact.PurgeCondition = purgeCondition;
                        return fact;
                    });
            return factRule;
        }

        public static Rule<Symbol, Set> GetSetRule()
        {
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
            return setRule;
        }

        private static Rule<Symbol, Namespace> Rule()
        {
            var namespaceRule = GetNamespaceRule();
            var factRule = GetFactRule();

            // factual_file -> namespace_declaration fact*
            var rule = Many(namespaceRule, factRule, (namespaceRoot, fact) => namespaceRoot.AddFact(fact));
            return Sequence(
                rule,
                Terminal(Symbol.EndOfFile), "Declare a fact.",
                (n, eof) => n);
        }

        private static Query CreateQuery(DataType type, Token<Symbol> nameToken)
        {
            DataTypeFact factType = type as DataTypeFact;
            if (factType == null)
                throw new ParserException("A query must return a fact type, not a native type.", type.LineNumber);
            if (factType.Cardinality != Cardinality.Many)
                throw new ParserException("A query must return multiple results.", factType.LineNumber);

            return new Query(nameToken.Value, factType.FactName, factType.LineNumber);
        }

        private static FactMember CreateField(DataType dataType, string name, bool publish, Condition publishCondition)
        {
            Field field = new Field(dataType.LineNumber, name, dataType, publish, publishCondition);
            return field;
        }
    }
}
