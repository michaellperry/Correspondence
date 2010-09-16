using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class FieldTest : TestBase
    {
        [TestMethod]
        public void WhenNativeFieldIsFound_FieldIsCreated()
        {
            Analyzed result = AssertNoError(
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
            Analyzed result = AssertNoError(
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
                    Has<Predecessor>.Property(p => p.Cardinality, Is.EqualTo(Cardinality.One)) &
                    Has<Predecessor>.Property(p => p.IsPivot, Is.EqualTo(false))
                ))
            ));
        }

        [TestMethod]
        public void WhenFactFieldIsPivot_PredecessorIsPivot()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  publish GameQueue gameQueue;\r\n" +
                "}"
            );

            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Predecessors, Contains<Predecessor>.That(
                    Has<Predecessor>.Property(p => p.Name, Is.EqualTo("gameQueue")) &
                    Has<Predecessor>.Property(p => p.FactType, Is.EqualTo("GameQueue")) &
                    Has<Predecessor>.Property(p => p.Cardinality, Is.EqualTo(Cardinality.One)) &
                    Has<Predecessor>.Property(p => p.IsPivot, Is.EqualTo(true))
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
    }
}
