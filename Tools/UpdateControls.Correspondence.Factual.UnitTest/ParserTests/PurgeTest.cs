using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class PurgeTest : TestBase
    {
        [TestMethod]
        public void WhenNoPurgeClause_FactIsNotPurged()
        {
            Namespace result = AssertNoErrors(
                "namespace Chess;       " +
                "fact Game {            " +
                "key:                   " +
                "  unique;              " +
                "}                      ");

            Fact game = result.WithFactNamed("Game");
            Assert.IsNull(game.PurgeCondition);
        }

        [TestMethod]
        public void WhenPurgeClause_FactIsPurged()
        {
            Namespace result = AssertNoErrors(
                "namespace Chess;             " +
                "fact Game {                  " +
                "purge where this.isFinished; " +
                "key:                         " +
                "  unique;                    " +
                "}                            ");

            Fact game = result.WithFactNamed("Game");
            Assert.IsNotNull(game.PurgeCondition);
            Assert.AreEqual(1, game.PurgeCondition.Clauses.Count());
            var clause = game.PurgeCondition.Clauses.Single();
            Assert.AreEqual("isFinished", clause.PredicateName);
            Assert.AreEqual(ConditionModifier.Positive, clause.Existence);
        }
    }
}
