using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.AST;
using QEDCode.LLOne;

namespace Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class FactTest : TestBase
    {
        [TestMethod]
        public void WhenFactIsGiven_FactIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel;\r\n" +
                "                            \r\n" +
                "fact GameQueue {key:}       \r\n";
            Namespace result = ParseToNamespace(code);
            Fact gameQueue = result.WithFactNamed("GameQueue");
            Assert.AreEqual(3, gameQueue.LineNumber);
        }

        [TestMethod]
        public void WhenFactHasField_FieldIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel;\r\n" +
                "                            \r\n" +
                "fact GameQueue {            \r\n" +
                "key:                        \r\n" +
                "  string identifier;        \r\n" +
                "}                           \r\n";
            Namespace result = ParseToNamespace(code);
            Field identifier = result.WithFactNamed("GameQueue").WithFieldNamed("identifier");
            Assert.AreEqual(5, identifier.LineNumber);
            DataTypeNative type = identifier.Type.ThatIsDataTypeNative();
            Assert.AreEqual(Cardinality.One, type.Cardinality);
            Assert.AreEqual(NativeType.String, type.NativeType);
        }
    }
}
