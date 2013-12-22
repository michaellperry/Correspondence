using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class UniqueTest : TestBase
    {
        [TestMethod]
        public void WhenUnique_UniqueIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel; " +
                "                             " +
                "fact Person {                " +
                "key:                         " +
                "    unique;                  " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            Fact person = result.WithFactNamed("Person");
            Assert.IsTrue(person.Unique, "The Person fact is not unique.");
        }
    }
}
