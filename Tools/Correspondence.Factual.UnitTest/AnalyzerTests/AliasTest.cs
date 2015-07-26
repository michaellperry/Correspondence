using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class AliasTest : TestBase
    {
        [TestMethod]
        public void AliasChangesClassName()
        {
            string code =
                "namespace model; " +
                "fact Fact 1 as FactV1 { key: }";
            var analyzed = AssertNoError(code);

            analyzed.HasClassNamed("FactV1");
        }

        [TestMethod]
        public void AliasDoesNotChangeSerializedName()
        {
            string code =
                "namespace model; " +
                "fact Fact 1 as FactV1 { key: }";
            var analyzed = AssertNoError(code);

            var fact = analyzed.HasClassNamed("FactV1");
            Assert.AreEqual("Fact", fact.SerializedName);
        }
    }
}
