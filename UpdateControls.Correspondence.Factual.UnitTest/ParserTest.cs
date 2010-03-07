using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
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
            try
            {
                FactualParser parser = new FactualParser(new StringReader(""));
                Namespace result = parser.Parse();
                Assert.Fail("A FactualException should have been thrown.");
            }
            catch (FactualException ex)
            {
                Pred.Assert(ex.Message, Is.EqualTo("Add a 'namespace' declaration."));
                Pred.Assert(ex.LineNumber, Is.EqualTo(1));
            }
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_DottedIdentifierIsRequired()
        {
            try
            {
                FactualParser parser = new FactualParser(new StringReader("namespace"));
                Namespace result = parser.Parse();
                Assert.Fail("A FactualException should have been thrown.");
            }
            catch (FactualException ex)
            {
                Pred.Assert(ex.Message, Is.EqualTo("Provide a dotted identifier for the namespace."));
                Pred.Assert(ex.LineNumber, Is.EqualTo(1));
            }
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_SemicolonIsRequired()
        {
            try
            {
                FactualParser parser = new FactualParser(new StringReader("namespace Reversi.GameModel"));
                Namespace result = parser.Parse();
                Assert.Fail("A FactualException should have been thrown.");
            }
            catch (FactualException ex)
            {
                Pred.Assert(ex.Message, Is.EqualTo("Terminate the namespace declaration with a semicolon."));
                Pred.Assert(ex.LineNumber, Is.EqualTo(1));
            }
        }

        [TestMethod]
        public void WhenNamespaceHasNoDot_NamepaceIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace GameModel;"));
            Namespace result = parser.Parse();
            Pred.Assert(result.Identifier, Is.EqualTo("GameModel"));
            Pred.Assert(result.LineNumber, Is.EqualTo(1));
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_NamepaceIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader("namespace Reversi.GameModel;"));
            Namespace result = parser.Parse();
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
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")).And(
                Has<Fact>.Property(fact => fact.LineNumber, Is.EqualTo(3)))));
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
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")).And(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("identifier")).And(
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)).And(
                        KindOf<DataType, DataTypeNative>.That(
                            Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String)))
                    )).And(
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))))
                )))
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
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("gameQueue")).And(
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)).And(
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("GameQueue")))
                    )).And(
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))))
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
            Namespace result = parser.Parse();
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
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Property>(), Contains<Property>.That(
                    Has<Property>.Property(property => property.Name, Is.EqualTo("firstName")).And(
                    Has<Property>.Property(property => property.Type, KindOf<DataType, DataTypeNative>.That(
                        Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String))
                    )))
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
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Query>(), Contains<Query>.That(
                    Has<Query>.Property(query => query.Name, Is.EqualTo("addresses")).And(
                    Has<Query>.Property(query => query.FactName, Is.EqualTo("Address"))).And(
                    Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                        Has<Set>.Property(set => set.Name, Is.EqualTo("address")).And(
                        Has<Set>.Property(set => set.FactName, Is.EqualTo("Address"))).And(
                        Has<Set>.Property(set => set.LeftPath, KindOf<AST.Path, PathRelative>.That(
                            Has<PathRelative>.Property(path => path.Segments,
                                Contains<string>.That(Is.EqualTo("address")).And(
                                Contains<string>.That(Is.EqualTo("person")))
                            )
                        ))).And(
                        Has<Set>.Property(set => set.RightPath, KindOf<AST.Path, PathAbsolute>.That(Is.NotNull<PathAbsolute>())))
                    )))
                ))
            ));
        }
    }
}
