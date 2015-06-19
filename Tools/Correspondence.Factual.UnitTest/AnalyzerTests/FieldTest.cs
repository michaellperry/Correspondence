using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.Compiler;
using Correspondence.Factual.Metadata;
using System.Linq;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class FieldTest : TestBase
    {
        [TestMethod]
        public void WhenNativeFieldIsFound_FieldIsCreated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "  string identifier;\r\n" +
                "}"
            );

            var field = result
                .HasClassNamed("GameQueue")
                .HasFieldNamed("identifier");
            Assert.AreEqual(NativeType.String, field.NativeType);
            Assert.AreEqual(Cardinality.One, field.Cardinality);
        }

        [TestMethod]
        public void WhenFactFieldIsFound_PredecessorIsCreated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "  string identifier;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            var predecessor = result
                .HasClassNamed("GameRequest")
                .HasPredecessorNamed("gameQueue");
            Assert.AreEqual("GameQueue", predecessor.FactType);
            Assert.AreEqual(Cardinality.One, predecessor.Cardinality);
            Assert.AreEqual(false, predecessor.IsPivot);
        }

        [TestMethod]
        public void WhenFactFieldIsPivot_PredecessorIsPivot()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "  string identifier;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  publish GameQueue gameQueue;\r\n" +
                "}"
            );

            var predecessor = result
                .HasClassNamed("GameRequest")
                .HasPredecessorNamed("gameQueue");
            Assert.AreEqual("GameQueue", predecessor.FactType);
            Assert.AreEqual(Cardinality.One, predecessor.Cardinality);
            Assert.AreEqual(true, predecessor.IsPivot);
        }

        [TestMethod]
        public void WhenFieldIsDuplicated_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Id { key: }\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "  string identifier;\r\n" +
                "  Id identifier;\r\n" +
                "}"
            );

            Assert.AreEqual(2, errors.Count());
            var error7 = errors.ElementAt(0);
            var error8 = errors.ElementAt(1);
            Assert.AreEqual("The member \"GameQueue.identifier\" is defined more than once.", error7.Message);
            Assert.AreEqual("The member \"GameQueue.identifier\" is defined more than once.", error8.Message);
            Assert.AreEqual(7, error7.LineNumber);
            Assert.AreEqual(8, error8.LineNumber);
        }

        [TestMethod]
        public void WhenFieldIsANativeType_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.identifier = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  string identifier;\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The member \"GameRequest.identifier\" is not a fact.", error.Message);
            Assert.AreEqual(7, error.LineNumber);
        }
    }
}
