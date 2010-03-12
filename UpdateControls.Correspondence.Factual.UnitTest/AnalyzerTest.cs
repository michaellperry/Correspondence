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
        public void WhenFactIsFound_ClassIsCreated()
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
            Pred.Assert(result.Name, Is.EqualTo("Reversi.GameModel"));
            Pred.Assert(result.Classes, Contains<Class>.That(Has<Class>.Property(c => c.Name, Is.EqualTo("GameQueue"))));
        }
    }
}
