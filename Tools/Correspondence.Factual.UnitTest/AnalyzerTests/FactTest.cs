using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.Compiler;
using Correspondence.Factual.Metadata;
using System.Linq;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class FactTest : TestBase
    {
        [TestMethod]
        public void WhenNamespaceIsFound_NamespaceIsCreated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;"
            );

            Assert.AreEqual("Reversi.GameModel", result.Name);
            Assert.AreEqual(0, result.Classes.Count());
        }

        [TestMethod]
        public void WhenFactIsFound_ClassIsCreated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { key: }"
            );

            Assert.AreEqual("Reversi.GameModel", result.Name);
            result.HasClassNamed("GameQueue");
        }

        [TestMethod]
        public void WhenFactIsDuplicated_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { key: }\r\n" +
                "fact GameQueue { key: }"
            );

            Assert.AreEqual(2, errors.Count());
            var error3 = errors.ElementAt(0);
            var error4 = errors.ElementAt(1);
            Assert.AreEqual("The fact \"GameQueue\" is defined more than once.", error3.Message);
            Assert.AreEqual("The fact \"GameQueue\" is defined more than once.", error4.Message);
            Assert.AreEqual(3, error3.LineNumber);
            Assert.AreEqual(4, error4.LineNumber);
        }
    }
}
