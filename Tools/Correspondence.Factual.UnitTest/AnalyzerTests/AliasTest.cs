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

        [TestMethod]
        public void AliasChangesMutableClassName()
        {
            string code =
                "namespace model; " +
                "fact Fact { " +
                "key: " +
                "mutable: " +
                "  int property as MyProperty; " +
                "}";
            var analyzed = AssertNoError(code);

            var fact = analyzed.HasClassNamed("MyProperty");
        }

        [TestMethod]
        public void AliasDoesNotChangeMutableSerializedName()
        {
            string code =
                "namespace model; " +
                "fact Fact { " +
                "key: " +
                "mutable: " +
                "  int property as MyProperty; " +
                "}";
            var analyzed = AssertNoError(code);

            var fact = analyzed.HasClassNamed("MyProperty");
            Assert.AreEqual("Fact__property", fact.SerializedName);
        }

        [TestMethod]
        public void FactAliasChangesMutableClassName()
        {
            string code =
                "namespace model; " +
                "fact Fact as MyFact { " +
                "key: " +
                "mutable: " +
                "  int property; " +
                "}";
            var analyzed = AssertNoError(code);

            var fact = analyzed.HasClassNamed("MyFact__property");
        }

        [TestMethod]
        public void FactAliasDoesNotChangeMutableSerializedName()
        {
            string code =
                "namespace model; " +
                "fact Fact as MyFact { " +
                "key: " +
                "mutable: " +
                "  int property; " +
                "}";
            var analyzed = AssertNoError(code);

            var fact = analyzed.HasClassNamed("MyFact__property");
            Assert.AreEqual("Fact__property", fact.SerializedName);
        }
    }
}
