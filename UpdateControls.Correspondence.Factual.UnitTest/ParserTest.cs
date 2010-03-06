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
            Parser parser = new Parser(new StringReader("namespace Reversi.GameModel;\r\n\r\nfact GameQueue {}"));
            Namespace result = parser.Parse();
            Pred.Assert(result.Facts, Contains<Fact>.That(Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue"))));
        }
    }
}
