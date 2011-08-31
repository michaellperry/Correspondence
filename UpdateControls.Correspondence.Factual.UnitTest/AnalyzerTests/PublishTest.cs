using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class PublishTest : TestBase
    {
        [TestMethod]
        public void PublishWithPositiveNegativeCondition()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameQueue { key: }        " +
                "                               " +
                "fact Completed {               " +
                "key:                           " +
                "  GameRequest gameRequest;     " +
                "}                              " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where this.isOngoing;      " +
                "                               " +
                "query:                         " +
                "  bool isOngoing {             " +
                "    not exists Completed c :   " +
                "      c.gameRequest = this     " +
                "  }                            " +
                "}                              ";
            Analyzed analyzed = AssertNoError(code);
            Predecessor gameQueue = analyzed.HasClassNamed("GameRequest").HasPredecessorNamed("gameQueue");
            Assert.IsTrue(gameQueue.IsPivot);
            Condition publishCondition = gameQueue.PublishConditions.Single();
            Assert.IsNotNull(publishCondition);
            Assert.AreEqual("isOngoing", publishCondition.Name);
            Assert.AreEqual(ConditionModifier.Negative, publishCondition.Modifier);
            Assert.AreEqual("GameRequest", publishCondition.Type);
        }

        [TestMethod]
        public void PublishWithNegativePositiveCondition()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameQueue { key: }        " +
                "                               " +
                "fact Completed {               " +
                "key:                           " +
                "  GameRequest gameRequest;     " +
                "}                              " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where not this.isCompleted;" +
                "                               " +
                "query:                         " +
                "  bool isCompleted {           " +
                "    exists Completed c :       " +
                "      c.gameRequest = this     " +
                "  }                            " +
                "}                              ";
            Analyzed analyzed = AssertNoError(code);
            Predecessor gameQueue = analyzed.HasClassNamed("GameRequest").HasPredecessorNamed("gameQueue");
            Assert.IsTrue(gameQueue.IsPivot);
            Condition publishCondition = gameQueue.PublishConditions.Single();
            Assert.IsNotNull(publishCondition);
            Assert.AreEqual("isCompleted", publishCondition.Name);
            Assert.AreEqual(ConditionModifier.Negative, publishCondition.Modifier);
            Assert.AreEqual("GameRequest", publishCondition.Type);
        }

        [TestMethod]
        public void PublishWithNegativeNegativeCondition()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameQueue { key: }        " +
                "                               " +
                "fact Completed {               " +
                "key:                           " +
                "  GameRequest gameRequest;     " +
                "}                              " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where not this.isCompleted;" +
                "                               " +
                "query:                         " +
                "  bool isCompleted {           " +
                "    not exists Completed c :   " +
                "      c.gameRequest = this     " +
                "  }                            " +
                "}                              ";
            Analyzed analyzed = AssertNoError(code);
            Predecessor gameQueue = analyzed.HasClassNamed("GameRequest").HasPredecessorNamed("gameQueue");
            Assert.IsTrue(gameQueue.IsPivot);
            Condition publishCondition = gameQueue.PublishConditions.Single();
            Assert.IsNotNull(publishCondition);
            Assert.AreEqual("isCompleted", publishCondition.Name);
            Assert.AreEqual(ConditionModifier.Positive, publishCondition.Modifier);
            Assert.AreEqual("GameRequest", publishCondition.Type);
        }

        [TestMethod]
        public void PublishWithPositivePositiveCondition()
        {
            string code =
                "namespace Reversi.GameModel;   " +
                "                               " +
                "fact GameQueue { key: }        " +
                "                               " +
                "fact Completed {               " +
                "key:                           " +
                "  GameRequest gameRequest;     " +
                "}                              " +
                "                               " +
                "fact GameRequest {             " +
                "key:                           " +
                "  publish GameQueue gameQueue  " +
                "    where this.isOngoing;      " +
                "                               " +
                "query:                         " +
                "  bool isOngoing {             " +
                "    exists Completed c :       " +
                "      c.gameRequest = this     " +
                "  }                            " +
                "}                              ";
            Analyzed analyzed = AssertNoError(code);
            Predecessor gameQueue = analyzed.HasClassNamed("GameRequest").HasPredecessorNamed("gameQueue");
            Assert.IsTrue(gameQueue.IsPivot);
            Condition publishCondition = gameQueue.PublishConditions.Single();
            Assert.IsNotNull(publishCondition);
            Assert.AreEqual("isOngoing", publishCondition.Name);
            Assert.AreEqual(ConditionModifier.Positive, publishCondition.Modifier);
            Assert.AreEqual("GameRequest", publishCondition.Type);
        }
    }
}
