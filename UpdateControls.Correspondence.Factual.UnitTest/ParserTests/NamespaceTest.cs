using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using QEDCode.LLOne;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class NamespaceTest : TestBase
    {
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
    }
}
