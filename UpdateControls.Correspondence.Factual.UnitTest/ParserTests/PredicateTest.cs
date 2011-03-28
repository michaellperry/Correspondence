using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class PredicateTest : TestBase
    {
        [TestMethod]
        public void WhenPredicateIsNegative_ExistenceIsNegative()
        {
            string code =
                "namespace Reversi.GameModel;                            " +
                "                                                        " +
                "fact Request {                                          " +
                "key:                                                    " +
                "query:                                                  " +
                "	bool isOutstanding {                                 " +
                "		not exists Accept accept : accept.request = this " +
                "	}                                                    " +
                "}                                                       ";
            Namespace result = ParseToNamespace(code);
            Predicate isOutstanding = result.WithFactNamed("Request").WithPredicateNamed("isOutstanding");
            Assert.AreEqual(ConditionModifier.Negative, isOutstanding.Existence);
            Set set = isOutstanding.Sets.Single();
            Assert.AreEqual("accept", set.Name);
            Assert.AreEqual("Accept", set.FactName);
        }

        [TestMethod]
        public void WhenPredicateIsPositive_ExistenceIsPositive()
        {
            string code =
                "namespace Reversi.GameModel;                        " +
                "                                                    " +
                "fact Person {                                       " +
                "key:                                                " +
                "query:                                              " +
                "	bool isPlaying {                                 " +
                "		exists Game game : game.player.person = this " +
                "	}                                                " +
                "}                                                   ";
            Namespace result = ParseToNamespace(code);
            Predicate isPlaying = result.WithFactNamed("Person").WithPredicateNamed("isPlaying");
            Assert.AreEqual(ConditionModifier.Positive, isPlaying.Existence);
            Set set = isPlaying.Sets.Single();
            Assert.AreEqual("game", set.Name);
            Assert.AreEqual("Game", set.FactName);
        }
    }
}
