using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using System.Linq;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class PredecessorTest : TestBase
    {
        [TestMethod]
        public void WhenPredecessorIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The fact type \"GameQueue\" is not defined.", error.Message);
            Assert.AreEqual(5, error.LineNumber);
        }

        [TestMethod]
        public void WhenPredecessorIsUndefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The member \"GameRequest.gameQueue\" is not defined.", error.Message);
            Assert.AreEqual(7, error.LineNumber);
        }
    }
}
