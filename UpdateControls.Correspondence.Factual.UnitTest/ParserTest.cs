using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using QEDCode.LLOne;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest
{
    [TestClass]
    public class ParserTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenInputIsEmpty_NamespaceIsRequired()
        {
            FactualParser parser = new FactualParser(new StringReader(""));
            Namespace result = parser.Parse();

            Pred.Assert(result, Is.Null<Namespace>());
            Pred.Assert(parser.Errors, Contains<ParserError>.That(
                Has<ParserError>.Property(e => e.Message, Is.EqualTo("Add a 'namespace' declaration.")) &
                Has<ParserError>.Property(e => e.LineNumber, Is.EqualTo(1))
            ));
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_DottedIdentifierIsRequired()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace"));
            Namespace result = parser.Parse();

            Pred.Assert(result, Is.Null<Namespace>());
            Pred.Assert(parser.Errors, Contains<ParserError>.That(
                Has<ParserError>.Property(e => e.Message, Is.EqualTo("Provide a dotted identifier for the namespace.")) &
                Has<ParserError>.Property(e => e.LineNumber, Is.EqualTo(1))
            ));
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_SemicolonIsRequired()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace Reversi.GameModel"));
            Namespace result = parser.Parse();

            Pred.Assert(result, Is.Null<Namespace>());
            Pred.Assert(parser.Errors, Contains<ParserError>.That(
                Has<ParserError>.Property(e => e.Message, Is.EqualTo("Terminate the namespace declaration with a semicolon.")) &
                Has<ParserError>.Property(e => e.LineNumber, Is.EqualTo(1))
            ));
        }

        [TestMethod]
        public void WhenNamespaceHasNoDot_NamepaceIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace GameModel;"));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Identifier, Is.EqualTo("GameModel"));
            Pred.Assert(result.LineNumber, Is.EqualTo(1));
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_NamepaceIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace Reversi.GameModel;"));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Identifier, Is.EqualTo("Reversi.GameModel"));
        }

        [TestMethod]
        public void WhenFactIsGiven_FactIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")) &
                Has<Fact>.Property(fact => fact.LineNumber, Is.EqualTo(3))
            ));
        }

        [TestMethod]
        public void WhenFactHasField_FieldIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")) &
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("identifier")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeNative>.That(
                            Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsPredecessor_PredecessorIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("gameQueue")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("GameQueue")) &
							Has<DataTypeFact>.Property(type => type.IsPivot, Is.EqualTo(false))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsPivot_PivotIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  pivot GameQueue gameQueue;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("gameQueue")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("GameQueue")) &
							Has<DataTypeFact>.Property(type => type.IsPivot, Is.EqualTo(true))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsOptional_CardinalityIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string? identifier;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Field>(), Contains<Field>.That(
                    Has<Field>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.Optional)))
                ))
            ));
        }

        [TestMethod]
        public void WhenFactHasProperty_PropertyIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  property string firstName;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Property>(), Contains<Property>.That(
                    Has<Property>.Property(property => property.Name, Is.EqualTo("firstName")) &
                    Has<Property>.Property(property => property.Type, KindOf<DataType, DataTypeNative>.That(
                        Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String))
                    ))
                ))
            ));
        }

        [TestMethod]
        public void WhenFactHasQuery_QueryIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(KindOf<FactMember, Query>.That(
                    Has<Query>.Property(query => query.Name, Is.EqualTo("addresses")) &
                    Has<Query>.Property(query => query.FactName, Is.EqualTo("Address")) &
                    Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                        Has<Set>.Property(set => set.Name, Is.EqualTo("address")) &
                        Has<Set>.Property(set => set.FactName, Is.EqualTo("Address")) &
                        Has<Set>.Property(set => set.LeftPath,
                            Has<AST.Path>.Property(path => path.Absolute, Is.EqualTo(false)) &
                            Has<AST.Path>.Property(path => path.RelativeTo, Is.EqualTo("address")) &
                            Has<AST.Path>.Property(path => path.Segments,
                                Contains<string>.That(Is.EqualTo("person"))
                            )
                        ) &
                        Has<Set>.Property(set => set.RightPath,
                            Has<AST.Path>.Property(path => path.Absolute, Is.EqualTo(true))
                        )
                    ))
                )))
            ));
        }

        [TestMethod]
        public void WhenMultiLineCommentFound_CommentIsIgnored()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "/*fact Ignored\r\n" +
                "{\r\n" +
                "}\r\n" +
                "*/\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, ContainsNo<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Ignored"))
            ));
        }

        [TestMethod]
        public void WhenSingleLineCommentFound_CommentIsIgnored()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "  //string ignored;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, ContainsNo<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("ignored"))
                ))
            ));
        }

        [TestMethod]
        public void WhenPredicateIsNegative_ExistenceIsNegative()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "	Frame frame;\r\n" +
                "	Person requester;\r\n" +
                "	\r\n" +
                "	bool isOutstanding {\r\n" +
                "		not exists Accept accept : accept.request = this\r\n" +
                "	}\r\n" +
                "	\r\n" +
                "	Bid* bids {\r\n" +
                "		Bid bid : bid.request = this\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("isOutstanding")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Predicate>.That(
                        Has<Predicate>.Property(predicate => predicate.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                        Has<Predicate>.Property(predicate => predicate.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("accept")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Accept"))
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenPredicateIsPositive_ExistenceIsPositive()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("isPlaying")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(4)) &
                    KindOf<FactMember, Predicate>.That(
                        Has<Predicate>.Property(predicate => predicate.Existence, Is.EqualTo(ConditionModifier.Positive)) &
                        Has<Predicate>.Property(predicate => predicate.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("game")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Game"))
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenWhere_ConditionHasClause()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "	Queue queue;\r\n" +
                "	Time timestamp;\r\n" +
                "	\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted /*and not request.isCanceled*/\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("outstandingRequests")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Query>.That(
                        Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("request")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Request")) &
                            Has<Set>.Property(set => set.Condition,
                                Has<Condition>.Property(condition => condition.Clauses, Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isAccepted"))
                                ))
                            )
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenWhereAndAnd_ConditionHasTwoClauses()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "	Queue queue;\r\n" +
                "	Time timestamp;\r\n" +
                "	\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted and not request.isCanceled\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("outstandingRequests")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Query>.That(
                        Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("request")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Request")) &
                            Has<Set>.Property(set => set.Condition,
                                Has<Condition>.Property(condition => condition.Clauses, Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isAccepted"))
                                ) & Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isCanceled"))
                                ))
                            )
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenUnique_UniqueIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "    unique;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Person")) &
                Has<Fact>.Property(fact => fact.Unique, Is.EqualTo(true))
            ));
        }

        private static Namespace AssertNoErrors(FactualParser parser)
        {
            Namespace result = parser.Parse();
            if (result == null)
                Assert.Fail(parser.Errors.First().Message);
            return result;
        }
    }
}
