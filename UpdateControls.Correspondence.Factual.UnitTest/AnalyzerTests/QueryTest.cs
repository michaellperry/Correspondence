using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
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

            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Name, Is.EqualTo("GameQueue")) &
                Has<Class>.Property(c => c.Queries, Contains<Query>.That(
                    Has<Query>.Property(q => q.Name, Is.EqualTo("gameRequests")) &
                    Has<Query>.Property(q => q.Joins, Contains<Join>.That(
                        Has<Join>.Property(j => j.Direction, Is.EqualTo(Direction.Successors)) &
                        Has<Join>.Property(j => j.Type, Is.EqualTo("GameRequest")) &
                        Has<Join>.Property(j => j.Name, Is.EqualTo("gameQueue"))
                    ))
                ))
            ));
        }

        [TestMethod]
        public void WhenQueryIsDefined_ResultIsGenerated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
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

            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Name, Is.EqualTo("GameQueue")) &
                Has<Class>.Property(c => c.Results, Contains<Result>.That(
                    Has<Result>.Property(r => r.Type, Is.EqualTo("GameRequest")) &
                    Has<Result>.Property(r => r.Query,
                        Has<Query>.Property(q => q.Name, Is.EqualTo("gameRequests"))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenQueryIsDefined_TypeMustExist()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
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

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The fact type \"TypeNotFound\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(5))
            ));
        }

        [TestMethod]
        public void WhenQueryIsDefined_TypeMustMatchResult()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
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
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The query results in \"GameRequest\", not \"TypeFoundButWrong\".")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(5))
            ));
        }

        [TestMethod]
        public void WhenTwoSetsAreJoined_QueryZigZags()
        {
            Analyzed result = AssertNoError(
                "namespace MagazineSubscriptions;\r\n" +
                "\r\n" +
                "fact Subscriber {\r\n" +
                "query:\r\n" +
                "  Article* articles {\r\n" +
                "    Subscription subscription : subscription.subscriber = this\r\n" +
                "    Article article : article.magazine = subscription.magazine\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Magazine {\r\n" +
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

            Class subscriber = result.Classes.Single(c => c.Name == "Subscriber");
            Query articles = subscriber.Queries.Single(q => q.Name == "articles");
            Join[] joins = articles.Joins.ToArray();
            Pred.Assert(joins.Length, Is.EqualTo(3));
            Pred.Assert(joins[0].Direction, Is.EqualTo(Direction.Successors));
            Pred.Assert(joins[0].Type, Is.EqualTo("Subscription"));
            Pred.Assert(joins[0].Name, Is.EqualTo("subscriber"));
            Pred.Assert(joins[1].Direction, Is.EqualTo(Direction.Predecessors));
            Pred.Assert(joins[1].Type, Is.EqualTo("Subscription"));
            Pred.Assert(joins[1].Name, Is.EqualTo("magazine"));
            Pred.Assert(joins[2].Direction, Is.EqualTo(Direction.Successors));
            Pred.Assert(joins[2].Type, Is.EqualTo("Article"));
            Pred.Assert(joins[2].Name, Is.EqualTo("magazine"));
        }
    }
}
