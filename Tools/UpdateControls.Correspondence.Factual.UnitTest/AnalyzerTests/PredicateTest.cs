using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class PredicateTest : TestBase
    {
        [TestMethod]
        public void WhenPredicateTypeIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The fact type \"Game\" is not defined.", error.Message);
            Assert.AreEqual(7, error.LineNumber);
        }

        [TestMethod]
        public void WhenPredicateFieldIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Game {\r\n" +
                "key:\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The member \"Game.player\" is not defined.", error.Message);
            Assert.AreEqual(11, error.LineNumber);
        }

        [TestMethod]
        public void WhenWhere_ConditionMustBeDefined()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted /*and not request.isCanceled*/\r\n" +
                "	}\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "key:\r\n" +
                "   Frame frame;\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The member \"Request.isAccepted\" is not defined.", error.Message);
            Assert.AreEqual(8, error.LineNumber);
        }

        [TestMethod]
        public void WhenWhere_QueryHasCondition()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted /*and not request.isCanceled*/\r\n" +
                "	}\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "key:\r\n" +
                "   Frame frame;\r\n" +
                "\r\n" +
                "query:\r\n" +
                "   bool isAccepted {\r\n" +
                "      exists Accept accept : accept.request = this\r\n" +
                "   }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Accept{\r\n" +
                "key:\r\n" +
                "   Request request;\r\n" +
                "}"
            );

            Class subscriber = result.HasClassNamed("Frame");
            Query articles = subscriber.HasQueryNamed("outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            var join = joins.Single();
            Assert.AreEqual(Direction.Successors, join.Direction);
            Assert.AreEqual("Request", join.Type);
            Assert.AreEqual("frame", join.Name);
            var condition = join.Conditions.Single();
            Assert.AreEqual("isAccepted", condition.Name);
            Assert.AreEqual(ConditionModifier.Negative, condition.Modifier);
        }

        [TestMethod]
        public void WhenWhereAndAnd_QueryHasMultipleCondition()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted and not request.isCanceled\r\n" +
                "	}\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "key:\r\n" +
                "   Frame frame;\r\n" +
                "\r\n" +
                "query:\r\n" +
                "   bool isAccepted {\r\n" +
                "      exists Accept accept : accept.request = this\r\n" +
                "   }\r\n" +
                "   bool isCanceled {\r\n" +
                "      exists Cancel cancel : cancel.request = this\r\n" +
                "   }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Accept{\r\n" +
                "key:\r\n" +
                "   Request request;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Cancel{\r\n" +
                "key:\r\n" +
                "   Request request;\r\n" +
                "}"
            );

            Class subscriber = result.HasClassNamed("Frame");
            Query articles = subscriber.HasQueryNamed("outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            var join = joins.Single();
            Assert.AreEqual(Direction.Successors, join.Direction);
            Assert.AreEqual("Request", join.Type);
            Assert.AreEqual("frame", join.Name);
            var accepted = join.Conditions.ElementAt(0);
            Assert.AreEqual("isAccepted", accepted.Name);
            Assert.AreEqual(ConditionModifier.Negative, accepted.Modifier);
            var canceled = join.Conditions.ElementAt(0);
            Assert.AreEqual("isCanceled", canceled.Name);
            Assert.AreEqual(ConditionModifier.Negative, canceled.Modifier);
        }

        [TestMethod]
        public void WhenPredicateIsInverted_ConditionIsInverted()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where request.isNotAccepted and not request.isCanceled\r\n" +
                "	}\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "key:\r\n" +
                "   Frame frame;\r\n" +
                "\r\n" +
                "query:\r\n" +
                "   bool isNotAccepted {\r\n" +
                "      not exists Accept accept : accept.request = this\r\n" +
                "   }\r\n" +
                "   bool isCanceled {\r\n" +
                "      exists Cancel cancel : cancel.request = this\r\n" +
                "   }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Accept{\r\n" +
                "key:\r\n" +
                "   Request request;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Cancel{\r\n" +
                "key:\r\n" +
                "   Request request;\r\n" +
                "}"
            );

            Class subscriber = result.HasClassNamed("Frame");
            Query articles = subscriber.HasQueryNamed("outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            var join = joins.Single();
            Assert.AreEqual(Direction.Successors, join.Direction);
            Assert.AreEqual("Request", join.Type);
            Assert.AreEqual("frame", join.Name);
            var accepted = join.Conditions.ElementAt(0);
            Assert.AreEqual("isNotAccepted", accepted.Name);
            Assert.AreEqual(ConditionModifier.Positive, accepted.Modifier);
            var canceled = join.Conditions.ElementAt(0);
            Assert.AreEqual("isCanceled", canceled.Name);
            Assert.AreEqual(ConditionModifier.Negative, canceled.Modifier);
        }
    }
}
