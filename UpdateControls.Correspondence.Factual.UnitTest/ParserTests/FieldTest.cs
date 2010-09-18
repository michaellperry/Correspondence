using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class FieldTest : TestBase
    {
        [TestMethod]
        public void WhenFieldIsPredecessor_PredecessorIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel; " +
                "                             " +
                "fact GameRequest {           " +
                "  GameQueue gameQueue;       " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            Field gameQueue = result.WithFactNamed("GameRequest").WithFieldNamed("gameQueue");
            Assert.AreEqual(Cardinality.One, gameQueue.Type.Cardinality);
            DataTypeFact type = gameQueue.Type.ThatIsDataTypeFact();
            Assert.IsInstanceOfType(gameQueue.Type, typeof(DataTypeFact));
            Assert.AreEqual("GameQueue", type.FactName);
            Assert.IsFalse(type.IsPivot, "The gameQueue field is a pivot.");
        }

        [TestMethod]
        public void WhenFieldIsPivot_PivotIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameRequest {             " +
                "  publish GameQueue gameQueue; " +
                "}                              ";
            Namespace result = ParseToNamespace(code);
            DataTypeFact type = result.WithFactNamed("GameRequest").WithFieldNamed("gameQueue")
                .Type.ThatIsDataTypeFact();
            Assert.AreEqual(Cardinality.One, type.Cardinality);
            Assert.IsTrue(type.IsPivot, "The gameQueue field is not a pivot.");
        }

        [TestMethod]
        public void WhenListIsPivot_PivotIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel; " +
                "                             " +
                "fact Game {                  " +
                "  publish User *players;     " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            DataTypeFact type = result.WithFactNamed("Game").WithFieldNamed("players")
                .Type.ThatIsDataTypeFact();
            Assert.AreEqual(Cardinality.Many, type.Cardinality);
            Assert.IsTrue(type.IsPivot, "The players field is not a pivot.");
        }

        [TestMethod]
        public void WhenFieldIsOptional_CardinalityIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel; " +
                "                             " +
                "fact GameQueue {             " +
                "  string? identifier;        " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            DataTypeNative type = result.WithFactNamed("GameQueue").WithFieldNamed("identifier")
                .Type.ThatIsDataTypeNative();
            Assert.AreEqual(Cardinality.Optional, type.Cardinality);
        }
    }
}
