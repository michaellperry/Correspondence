using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class VersionTest : TestBase
    {
        [TestMethod]
        public void FileWithNoHeaderHasNullVersion()
        {
            string code =
                "namespace model;";
            var result = ParseToNamespace(code);
            Assert.IsNull(result.Version);
        }

        [TestMethod]
        public void FileWithHeaderHasVersion()
        {
            string code =
                "namespace model; " +
                "version legacy;  ";
            var result = ParseToNamespace(code);
            Assert.AreEqual("legacy", result.Version);
        }
    }
}