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
                Parser parser = new Parser(new StringReader(""));
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
                Parser parser = new Parser(new StringReader("namespace"));
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
                Parser parser = new Parser(new StringReader("namespace Reversi.GameModel"));
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
            Parser parser = new Parser(new StringReader("namespace GameModel;"));
            Namespace result = parser.Parse();
            Pred.Assert(result.Identifier, Is.EqualTo("GameModel"));
            Pred.Assert(result.LineNumber, Is.EqualTo(1));
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_NamepaceIsRecognized()
        {
            Parser parser = new Parser(new StringReader("namespace Reversi.GameModel;"));
            Namespace result = parser.Parse();
            Pred.Assert(result.Identifier, Is.EqualTo("Reversi.GameModel"));
        }

        [TestMethod]
        public void WhenFactIsGiven_FactIsRecognized()
        {
            Parser parser = new Parser(new StringReader(
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
            Parser parser = new Parser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}"
            ));
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")).And(
                Has<Fact>.Property(fact => fact.Fields, Contains<Field>.That(
                    Has<Field>.Property(field => field.Name, Is.EqualTo("identifier")).And(
                    Has<Field>.Property(field => field.Type,
                        Has<FieldType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)).And(
                        KindOf<FieldType, FieldTypeNative>.That(
                            Has<FieldTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String)))
                    )).And(
                    Has<Field>.Property(field => field.LineNumber, Is.EqualTo(4))))
                )))
            ));
        }

        [TestMethod]
        public void WhenFieldIsOptional_CardinalityIsRecognized()
        {
            Parser parser = new Parser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string? identifier;\r\n" +
                "}"
            ));
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Fields, Contains<Field>.That(
                    Has<Field>.Property(field => field.Type,
                        Has<FieldType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.Optional)))
                ))
            ));
        }
    }
}
