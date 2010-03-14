using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using System.IO;
using UpdateControls.Correspondence.Factual.Metadata;
using Predassert;

namespace UpdateControls.Correspondence.Factual.UnitTest
{
    [TestClass]
    public class AnalyzerTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhenNamespaceIsFound_NamespaceIsCreated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Is.Empty<Class>());
        }

        [TestMethod]
        public void WhenFactIsFound_ClassIsCreated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { }"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Contains<Class>.That(Has<Class>.Property(c => c.Name, Is.EqualTo("GameQueue"))));
        }

        [TestMethod]
        public void WhenFactIsDuplicated_ErrorIsGenerated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue { }\r\n" +
                "fact GameQueue { }"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result, Is.Null<Namespace>());
            Pred.Assert(analyzer.Errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact \"GameQueue\" has already been defined.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(4))
            ));
        }

        [TestMethod]
        public void WhenNativeFieldIsFound_FieldIsCreated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Fields, Contains<Field>.That(
                    Has<Field>.Property(f => f.Name, Is.EqualTo("identifier")) &
                    Has<Field>.Property(f => f.DataType, Is.EqualTo(NativeType.String)) &
                    Has<Field>.Property(f => f.Cardinality, Is.EqualTo(Cardinality.One))
                ))
            ));
        }

        [TestMethod]
        public void WhenFactFieldIsFound_PredecessorIsCreated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result.Classes, Contains<Class>.That(
                Has<Class>.Property(c => c.Predecessors, Contains<Predecessor>.That(
                    Has<Predecessor>.Property(p => p.Name, Is.EqualTo("gameQueue")) &
                    Has<Predecessor>.Property(p => p.FactType, Is.EqualTo("GameQueue")) &
                    Has<Predecessor>.Property(p => p.Cardinality, Is.EqualTo(Cardinality.One))
                ))
            ));
        }
 
        [TestMethod]
        public void WhenPredecessorIsNotDefined_ErrorIsGenerated()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            ));
            Analyzer analyzer = new Analyzer(parser.Parse());
            Namespace result = analyzer.Analyze();
            Pred.Assert(result, Is.Null<Namespace>());
            Pred.Assert(analyzer.Errors, Contains<Error>.That(
                Has<Error>.Property(e => e.Message, Is.EqualTo("The fact type \"GameQueue\" is not defined.")) &
                Has<Error>.Property(e => e.LineNumber, Is.EqualTo(4))
            ));
        }
    }
}
