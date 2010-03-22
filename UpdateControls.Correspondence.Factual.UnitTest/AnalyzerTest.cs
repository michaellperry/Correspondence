using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using System.IO;
using UpdateControls.Correspondence.Factual.Metadata;
using Predassert;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.UnitTest
{
    [TestClass]
    public class AnalyzerTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenNamespaceIsFound_NamespaceIsCreated()
        {
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;"
            );

            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Is.Empty<Class>());
        }

        [TestMethod]
        public void WhenFactIsFound_ClassIsCreated()
        {
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { }"
            );

            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Contains<Class>.That(Has<Class>.Property(c => c.Name, Is.EqualTo("GameQueue"))));
        }

        [TestMethod]
        public void WhenFactIsDuplicated_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { }\r\n" +
                "fact GameQueue { }"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact \"GameQueue\" is defined more than once.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(3))
            ));
            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact \"GameQueue\" is defined more than once.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(4))
            ));
        }

        [TestMethod]
        public void WhenNativeFieldIsFound_FieldIsCreated()
        {
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}"
            );

            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Fields, Contains<Field>.That(
                    Has<Field>.Property(f => f.Name, Is.EqualTo("identifier")) &
                    Has<Field>.Property(f => f.NativeType, Is.EqualTo(NativeType.String)) &
                    Has<Field>.Property(f => f.Cardinality, Is.EqualTo(Cardinality.One))
                ))
            ));
        }

        [TestMethod]
        public void WhenFactFieldIsFound_PredecessorIsCreated()
        {
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Predecessors, Contains<Predecessor>.That(
                    Has<Predecessor>.Property(p => p.Name, Is.EqualTo("gameQueue")) &
                    Has<Predecessor>.Property(p => p.FactType, Is.EqualTo("GameQueue")) &
                    Has<Predecessor>.Property(p => p.Cardinality, Is.EqualTo(Cardinality.One))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsDuplicated_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Id { }\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "  Id identifier;\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The member \"GameQueue.identifier\" is defined more than once.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(6))
            ));
            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The member \"GameQueue.identifier\" is defined more than once.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(7))
            ));
        }
 
        [TestMethod]
        public void WhenPredecessorIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact type \"GameQueue\" is not defined.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(4))
            ));
        }

        [TestMethod]
        public void WhenPredecessorIsUndefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(5)) &
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"GameRequest.gameQueue\" is not defined."))
            ));
        }

        [TestMethod]
        public void WhenFieldIsANativeType_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.identifier = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  string identifier;\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(5)) &
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"GameRequest.identifier\" is not a fact."))
            ));
        }

        [TestMethod]
        public void WhenQueryIsDefined_QueryIsGenerated()
        {
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
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
            Namespace result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  GameRequest *gameRequests {\r\n" +
                "    GameRequest r : r.gameQueue = this\r\n" +
                "  }\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
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
        public void WhenTwoSetsAreJoined_QueryZigZags()
        {
            Namespace result = AssertNoError(
                "namespace MagazineSubscriptions;\r\n" +
                "\r\n" +
                "fact Subscriber {\r\n" +
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
                "  Subscriber subscriber;\r\n" +
                "  Magazine magazine;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Article {\r\n" +
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

        [TestMethod]
        public void WhenPredicateTypeIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The fact type \"Game\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(5))
            ));
        }

        [TestMethod]
        public void WhenPredicateFieldIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Game {\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"Game.player\" is not defined.")) &
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(8))
            ));
        }

        private static Namespace AssertNoError(string factual)
        {
            Analyzer analyzer = CreateAnalyzer(factual);
            Namespace result = analyzer.Analyze();
            if (result == null)
                Assert.Fail(analyzer.Errors.First().Message);
            return result;
        }

        private static IEnumerable<Error> AssertError(string factual)
        {
            Analyzer analyzer = CreateAnalyzer(factual);
            Namespace result = analyzer.Analyze();
            Pred.Assert(result, Is.Null<Namespace>());
            List<Error> errors = analyzer.Errors;
            return errors;
        }

        private static Analyzer CreateAnalyzer(string factual)
        {
            FactualParser parser = new FactualParser(new StringReader(factual));
            var source = parser.Parse();
            if (source == null)
                Assert.Fail(parser.Errors.First().Message);
            return new Analyzer(source);
        }
    }
}
