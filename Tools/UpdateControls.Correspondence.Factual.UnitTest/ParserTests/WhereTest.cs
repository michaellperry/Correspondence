using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class WhereTest : TestBase
    {
        [TestMethod]
        public void WhenWhere_ConditionHasClause()
        {
            string code =
                "namespace Reversi.GameModel;                  " +
                "                                              " +
                "fact Frame {                                  " +
                "key:                                          " +
                "query:                                        " +
                "	Request* outstandingRequests {             " +
                "		Request request : request.frame = this " +
                "			where not request.isAccepted       " +
                "	}                                          " +
                "}                                             ";
            Namespace result = ParseToNamespace(code);
            Set set = result.WithFactNamed("Frame").WithQueryNamed("outstandingRequests").WithSetNamed("request");
            Clause clause = set.Condition.Clauses.Single();
            Assert.AreEqual(ConditionModifier.Negative, clause.Existence);
            Assert.AreEqual("request", clause.Name);
            Assert.AreEqual("isAccepted", clause.PredicateName);
        }

        [TestMethod]
        public void WhenWhereAndAnd_ConditionHasTwoClauses()
        {
            string code =
                "namespace Reversi.GameModel;                                       " +
                "                                                                   " +
                "fact Frame {                                                       " +
                "key:                                                               " +
                "query:                                                             " +
                "	Request* outstandingRequests {                                  " +
                "		Request request : request.frame = this                      " +
                "			where not request.isAccepted and not request.isCanceled " +
                "	}                                                               " +
                "}                                                                  ";
            Namespace result = ParseToNamespace(code);
            Set set = result.WithFactNamed("Frame").WithQueryNamed("outstandingRequests").WithSetNamed("request");
            Clause[] clauses = set.Condition.Clauses.ToArray();
            Assert.AreEqual(2, clauses.Length);
            Assert.AreEqual(ConditionModifier.Negative, clauses[0].Existence);
            Assert.AreEqual("request", clauses[0].Name);
            Assert.AreEqual("isAccepted", clauses[0].PredicateName);
            Assert.AreEqual(ConditionModifier.Negative, clauses[1].Existence);
            Assert.AreEqual("request", clauses[1].Name);
            Assert.AreEqual("isCanceled", clauses[1].PredicateName);
        }
    }
}
