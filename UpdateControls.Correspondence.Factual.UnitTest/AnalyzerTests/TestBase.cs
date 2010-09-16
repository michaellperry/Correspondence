using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QEDCode.LLOne;
using UpdateControls.Correspondence.Factual.Compiler;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    public class TestBase
    {
        protected static Namespace AssertNoError(string factual)
        {
            Analyzer analyzer = CreateAnalyzer(factual);
            Namespace result = analyzer.Analyze();
            if (result == null)
                Assert.Fail(analyzer.Errors.First().Message);
            return result;
        }

        protected static IEnumerable<Error> AssertError(string factual)
        {
            Analyzer analyzer = CreateAnalyzer(factual);
            Namespace result = analyzer.Analyze();
            if (result != null)
                Assert.Fail("The analyzer was supposed to return an error.");
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
