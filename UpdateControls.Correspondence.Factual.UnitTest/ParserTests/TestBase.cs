using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

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
    }
}
