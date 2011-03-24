using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class PredecessorTest : TestBase
    {
        [TestMethod]
        public void WhenPredecessorIsNotDefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "key:\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact type \"GameQueue\" is not defined.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(5))
            ));
        }

        [TestMethod]
        public void WhenPredecessorIsUndefined_ErrorIsGenerated()
        {
            IEnumerable<Error> errors = AssertError(
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
                "}"
            );

            Pred.Assert(errors, Contains<Error>.That(
                Has<Error>.Property(error => error.LineNumber, Is.EqualTo(6)) &
                Has<Error>.Property(error => error.Message, Is.EqualTo("The member \"GameRequest.gameQueue\" is not defined."))
            ));
        }
    }
}
