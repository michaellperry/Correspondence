using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.AST;
using Correspondence.Factual.Compiler;
using System;
using System.IO;
using QEDCode.LLOne;

namespace Correspondence.Factual.UnitTest.ParserTests
{
    public class TestBase
    {
        protected static Namespace AssertNoErrors(string code)
        {
            FactualParser parser = new FactualParser(new StringReader(code));
            return AssertNoErrors(parser);
        }

        protected static Namespace AssertNoErrors(FactualParser parser)
        {
            Namespace result = parser.Parse();
            if (result == null)
            {
                ParserError error = parser.Errors.First();
                Assert.Fail(String.Format("{0}: {1}", error.LineNumber, error.Message));
            }
            return result;
        }

        protected static Namespace ParseToNamespace(string code)
        {
            return AssertNoErrors(new FactualParser(new StringReader(code)));
        }

        protected static ParserError ParseToError(string code)
        {
            FactualParser parser = new FactualParser(new StringReader(code));
            Namespace result = parser.Parse();
            Assert.IsNull(result, "The parser should have raised an error.");
            return parser.Errors.Single();
        }
    }
}
