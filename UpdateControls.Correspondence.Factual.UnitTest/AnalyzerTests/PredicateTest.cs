using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
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

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The fact type \"Game\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(7))
            ));
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

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"Game.player\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(11))
            ));
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

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"Request.isAccepted\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(8))
            ));
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

            Class subscriber = result.Classes.Single(c => c.Name == "Frame");
            Query articles = subscriber.Queries.Single(q => q.Name == "outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            Pred.Assert(joins.Length, Is.EqualTo(1));
            Pred.Assert(joins[0].Direction, Is.EqualTo(Direction.Successors));
            Pred.Assert(joins[0].Type, Is.EqualTo("Request"));
            Pred.Assert(joins[0].Name, Is.EqualTo("frame"));
            Pred.Assert(joins[0].Conditions, Contains<Condition>.That(
                Has<Condition>.Property(c => c.Name, Is.EqualTo("isAccepted")) &
                Has<Condition>.Property(c => c.Modifier, Is.EqualTo(ConditionModifier.Negative))
            ));
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

            Class subscriber = result.Classes.Single(c => c.Name == "Frame");
            Query articles = subscriber.Queries.Single(q => q.Name == "outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            Pred.Assert(joins.Length, Is.EqualTo(1));
            Pred.Assert(joins[0].Direction, Is.EqualTo(Direction.Successors));
            Pred.Assert(joins[0].Type, Is.EqualTo("Request"));
            Pred.Assert(joins[0].Name, Is.EqualTo("frame"));
            Pred.Assert(joins[0].Conditions, Contains<Condition>.That(
                Has<Condition>.Property(c => c.Name, Is.EqualTo("isAccepted")) &
                Has<Condition>.Property(c => c.Modifier, Is.EqualTo(ConditionModifier.Negative))
            ) & Contains<Condition>.That(
                Has<Condition>.Property(c => c.Name, Is.EqualTo("isCanceled")) &
                Has<Condition>.Property(c => c.Modifier, Is.EqualTo(ConditionModifier.Negative))
            ));
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

            Class subscriber = result.Classes.Single(c => c.Name == "Frame");
            Query articles = subscriber.Queries.Single(q => q.Name == "outstandingRequests");
            Join[] joins = articles.Joins.ToArray();
            Pred.Assert(joins.Length, Is.EqualTo(1));
            Pred.Assert(joins[0].Direction, Is.EqualTo(Direction.Successors));
            Pred.Assert(joins[0].Type, Is.EqualTo("Request"));
            Pred.Assert(joins[0].Name, Is.EqualTo("frame"));
            Pred.Assert(joins[0].Conditions, Contains<Condition>.That(
                Has<Condition>.Property(c => c.Name, Is.EqualTo("isNotAccepted")) &
                Has<Condition>.Property(c => c.Modifier, Is.EqualTo(ConditionModifier.Negative))
            ) & Contains<Condition>.That(
                Has<Condition>.Property(c => c.Name, Is.EqualTo("isCanceled")) &
                Has<Condition>.Property(c => c.Modifier, Is.EqualTo(ConditionModifier.Negative))
            ));
        }
    }
}
