using Microsoft.VisualStudio.TestTools.UnitTesting;
using QEDCode.LLOne;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class NamespaceTest : TestBase
    {
        [TestMethod]
        public void WhenInputIsEmpty_NamespaceIsRequired()
        {
            string code = "";
            ParserError error = ParseToError(code);
            Assert.AreEqual("Add a 'namespace' declaration.", error.Message);
            Assert.AreEqual(1, error.LineNumber);
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_DottedIdentifierIsRequired()
        {
            string code = "namespace";
            ParserError error = ParseToError(code);
            Assert.AreEqual("Provide a dotted identifier for the namespace.", error.Message);
            Assert.AreEqual(1, error.LineNumber);
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_SemicolonIsRequired()
        {
            string code = "namespace Reversi.GameModel";
            ParserError error = ParseToError(code);
            Assert.AreEqual("Terminate the namespace declaration with a semicolon.", error.Message);
            Assert.AreEqual(1, error.LineNumber);
        }

        [TestMethod]
        public void WhenNamespaceHasNoDot_NamepaceIsRecognized()
        {
            string code = "namespace GameModel;";
            Namespace result = ParseToNamespace(code);
            Assert.AreEqual("GameModel", result.Identifier);
            Assert.AreEqual(1, result.LineNumber);
        }

        [TestMethod]
        public void WhenNamespaceIsGiven_NamepaceIsRecognized()
        {
            string code = "namespace Reversi.GameModel;";
            Namespace result = ParseToNamespace(code);
            Assert.AreEqual("Reversi.GameModel", result.Identifier);
        }
    }
}
