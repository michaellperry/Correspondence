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
                "namespace Reversi.GameModel;\n" +
                "                            \n" +
                "fact Person {               \n" +
                "key:                        \n" +
                "    unique;                 \n" +
                "}                           \n";
            Namespace result = ParseToNamespace(code);
            Fact person = result.WithFactNamed("Person");
            Assert.IsTrue(person.Unique, "The Person fact is not unique.");
        }

        [TestMethod]
        public void WhenTwoUniques_SyntaxError()
        {
            string code =
                "namespace Reversi.GameModel;\n" +
                "                            \n" +
                "fact Person {               \n" +
                "key:                        \n" +
                "    unique;                 \n" +
                "    unique;                 \n" +
                "}                           \n";

            var error = ParseToError(code);
            Assert.AreEqual(6, error.LineNumber);
            Assert.AreEqual("The unique modifier can only be applied once.", error.Message);
        }
    }
}
