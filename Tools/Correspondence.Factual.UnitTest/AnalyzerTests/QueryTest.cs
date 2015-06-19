using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.Compiler;
using Correspondence.Factual.Metadata;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class QueryTest : TestBase
    {
        [TestMethod]
        public void WhenQueryIsDefined_QueryIsGenerated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            var query = result
                .HasClassNamed("GameQueue")
                .HasQueryNamed("gameRequests");
            var join = query.Joins.Single();
            Assert.AreEqual(Direction.Successors, join.Direction);
            Assert.AreEqual("GameRequest", join.Type);
            Assert.AreEqual("gameQueue", join.Name);
        }

        [TestMethod]
        public void WhenQueryIsDefined_ResultIsGenerated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            var queryResult = result
                .HasClassNamed("GameQueue")
                .HasResultNamed("gameRequests");
            Assert.AreEqual("GameRequest", queryResult.Type);
        }

        [TestMethod]
        public void WhenQueryIsDefined_TypeMustExist()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  TypeNotFound *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The fact type \"TypeNotFound\" is not defined.", error.Message);
            Assert.AreEqual(6, error.LineNumber);
        }

        [TestMethod]
        public void WhenQueryIsDefined_TypeMustMatchResult()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  TypeFoundButWrong *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact TypeFoundButWrong {\r\n" +
                "key:\r\n" +
                "}"
            );

            var error = errors.Single();
            Assert.AreEqual("The query results in \"GameRequest\", not \"TypeFoundButWrong\".", error.Message);
            Assert.AreEqual(6, error.LineNumber);
        }

        [TestMethod]
        public void WhenTwoSetsAreJoined_QueryZigZags()
        {
            Analyzed result = AssertNoError(
                "namespace MagazineSubscriptions;\r\n" +
                "\r\n" +
                "fact Subscriber {\r\n" +
                "key:\r\n" +
                "query:\r\n" +
                "  Article* articles {\r\n" +
                "    Subscription subscription : subscription.subscriber = this\r\n" +
                "    Article article : article.magazine = subscription.magazine\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Magazine {\r\n" +
                "key:\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Subscription {\r\n" +
                "key:\r\n" +
                "  Subscriber subscriber;\r\n" +
                "  Magazine magazine;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Article {\r\n" +
                "key:\r\n" +
                "  Magazine magazine;\r\n" +
                "}"
            );

            Class subscriber = result.HasClassNamed("Subscriber");
            Query articles = subscriber.HasQueryNamed("articles");
            Join[] joins = articles.Joins.ToArray();
            Assert.AreEqual(3, joins.Length);
            Assert.AreEqual(Direction.Successors  ,joins[0].Direction );
            Assert.AreEqual("Subscription"        ,joins[0].Type      );
            Assert.AreEqual("subscriber"          ,joins[0].Name      );
            Assert.AreEqual(Direction.Predecessors,joins[1].Direction );
            Assert.AreEqual("Subscription"        ,joins[1].Type      );
            Assert.AreEqual("magazine"            ,joins[1].Name      );
            Assert.AreEqual(Direction.Successors  ,joins[2].Direction );
            Assert.AreEqual("Article"             ,joins[2].Type      );
            Assert.AreEqual("magazine"            ,joins[2].Name      );
        }

        private const string ProjectModel =
            "namespace Project.Model; " +
            "" +
            "fact Identity { " +
            "key: " +
                "unique; " +
                "" +
            "query: " +
                "ProjectShare* activeProjectShares { " +
                    "ProjectShare s : s.identity = this " +
                        "where s.isActive " +
                "} " +
                "" +
                "Project* activeProjects { " +
                    "ProjectShare s : s.identity = this " +
                        "where s.isActive " +
                    "Project p : p = s.project " +
                        "where not p.isCompleted " +
                "} " +
            "} " +
                "" +
            "fact ProjectShare { " +
            "key: " +
                "unique; " +
                "Identity identity; " +
                "Project project; " +
                "" +
            "query: " +
                "bool isActive { " +
                    "not exists ProjectShareRevoke r : r.projectShare = this " +
                "} " +
            "} " +
                "" +
            "fact ProjectShareRevoke { " +
            "key: " +
                "ProjectShare projectShare; " +
            "} " +
                "" +
            "fact Project { " +
            "key: " +
                "unique; " +
                "" +
            "query: " +
                "bool isCompleted { " +
                    "exists ProjectCompleted pc : pc.project = this " +
                "} " +
            "} " +
            " " +
            "fact ProjectCompleted { " +
            "key: " +
                "Project project; " +
                "date completionDate; " +
            "}";
            

        [TestMethod]
        public void WhenSuccessorHasCondition_TypeIsSuccessor()
        {
            Analyzed result = AssertNoError(ProjectModel);

            Query query = result.HasClassNamed("Identity").HasQueryNamed("activeProjectShares");
            Assert.AreEqual("ProjectShare", query.Joins.Single().Conditions.Single().Type);
        }

        [TestMethod]
        public void WhenPredecessorHasCondition_TypeIsPredecessor()
        {
            Analyzed result = AssertNoError(ProjectModel);

            Query query = result.HasClassNamed("Identity").HasQueryNamed("activeProjects");
            Assert.AreEqual("Project", query.Joins.ElementAt(1).Conditions.Single().Type);
        }
    }
}
