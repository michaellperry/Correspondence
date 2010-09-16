using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;
using System;
using System.IO;
using QEDCode.LLOne;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    public class TestBase
    {
        protected static Namespace AssertNoErrors(FactualParser parser)
        {
            Namespace result = parser.Parse();
            if (result == null)
                Assert.Fail(parser.Errors.First().Message);
            return result;
        }

        protected static Namespace AssertNoErrors(string code)
        {
            return AssertNoErrors(new FactualParser(new StringReader(code)));
        }
    }
}
