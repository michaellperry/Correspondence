using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.UnitTest.ParserTests;
using UpdateControls.Correspondence.Factual.UnitTest.Properties;

namespace UpdateControls.Correspondence.Factual.UnitTest.AcceptanceTests
{
    [TestClass]
    public class AsymmetricSecurityAcceptanceTest : TestBase
    {
        [TestMethod]
        public void CanParseSecureModel()
        {
            var ns = AssertNoErrors(Resources.AsymmetricSecurityModel);
        }
    }
}
