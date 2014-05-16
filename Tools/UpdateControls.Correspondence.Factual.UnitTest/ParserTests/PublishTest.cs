using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;
using QEDCode.LLOne;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class ConditionalPublishTest : TestBase
    {
        [TestMethod]
        public void WhenPublishIsPositiveConditional_ConditionIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where this.isOngoing;      " +
                "}                              ";
            Namespace result = ParseToNamespace(code);
            Field field = result.WithFactNamed("GameRequest").WithFieldNamed("gameQueue");
            Assert.IsTrue(field.Publish, "The gameQueue field is not a pivot.");
            Condition condition = field.PublishCondition;
            Assert.IsNotNull(condition);
            Assert.AreEqual(1, condition.Clauses.Count());
            Clause clause = condition.Clauses.Single();
            Assert.AreEqual(ConditionModifier.Positive, clause.Existence);
            Assert.AreEqual("this", clause.Name);
            Assert.AreEqual("isOngoing", clause.PredicateName);
        }

        [TestMethod]
        public void WhenPublishIsNegativeConditional_ConditionIsRecognized()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where not this.isFinished; " +
                "}                              ";
            Namespace result = ParseToNamespace(code);
            Field field = result.WithFactNamed("GameRequest").WithFieldNamed("gameQueue");
            Assert.IsTrue(field.Publish, "The gameQueue field is not a pivot.");
            Condition condition = field.PublishCondition;
            Assert.IsNotNull(condition);
            Assert.AreEqual(1, condition.Clauses.Count());
            Clause clause = condition.Clauses.Single();
            Assert.AreEqual(ConditionModifier.Negative, clause.Existence);
            Assert.AreEqual("this", clause.Name);
            Assert.AreEqual("isFinished", clause.PredicateName);
        }

        [TestMethod]
        public void MutableFieldsMayNotHavePublishConditions()
        {
            string code =
                "namespace ContactList;       \n" +
                "                             \n" +
                "fact Person {                \n" +
                "key:                         \n" +
                "mutable:                     \n" +
                "  publish string firstName   \n" +
                "    where this.isCurrent;    \n" +
                "}                            ";
            ParserError error = ParseToError(code);
            Assert.AreEqual(7, error.LineNumber);
            Assert.AreEqual("A mutable field may not have a publish condition.", error.Message);
        }
    }
}
