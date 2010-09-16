using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class FactTest : TestBase
    {
        [TestMethod]
        public void WhenNamespaceIsFound_NamespaceIsCreated()
        {
            Analyzed result = AssertNoError(
                "namespace Reversi.GameModel;"
            );

            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Is.Empty<Class>());
        }

        [TestMethod]
        public void WhenFactIsFound_ClassIsCreated()
        {
            Analyzed result = AssertNoError(
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
    }
}
