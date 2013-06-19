using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using Source = UpdateControls.Correspondence.Factual.AST;
using Target = UpdateControls.Correspondence.Factual.Metadata;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class MutableTest
    {
        private Target.Analyzed _analyzed;

        [TestInitialize]
        public void Initialize()
        {
            Source.Namespace root = new Source.Namespace("CRM.Model", 1, new List<Header>(), string.Empty)
                .AddFact(new Source.Fact("Customer", 3)
                    .AddMember(new Source.Property(5, "name", new Source.DataTypeNative(Source.NativeType.String, Source.Cardinality.One, 5), true))
                    .AddMember(new Source.Property(6, "employer", new Source.DataTypeFact("Company", Source.Cardinality.One, 6), false))
                );

            _analyzed = new Analyzer(root).Analyze();
        }

        [TestMethod]
        public void MutablePropertyCreatesAChildClass()
        {
            _analyzed.HasClassNamed("Customer__name");
        }

        [TestMethod]
        public void ChildClassHasParentPredecessor()
        {
            Assert.AreEqual(
                "Customer",
                _analyzed.HasClassNamed("Customer__name").HasPredecessorNamed("customer").FactType);
        }

        [TestMethod]
        public void ChildClassIsPublished()
        {
            Assert.IsTrue(
                _analyzed.HasClassNamed("Customer__name").HasPredecessorNamed("customer").IsPivot);
        }

        [TestMethod]
        public void OtherChildClassIsNotPublished()
        {
            Assert.IsFalse(
                _analyzed.HasClassNamed("Customer__employer").HasPredecessorNamed("customer").IsPivot);
        }

        [TestMethod]
        public void ChildClassHasPriorPredecessor()
        {
            Assert.AreEqual(
                "Customer__name",
                _analyzed.HasClassNamed("Customer__name").HasPredecessorNamed("prior").FactType);
        }

        [TestMethod]
        public void ChildClassHasValueField()
        {
            Assert.AreEqual(
                Target.NativeType.String,
                _analyzed.HasClassNamed("Customer__name").HasFieldNamed("value").NativeType);
        }

        [TestMethod]
        public void WhenMutableTypeIsNotNative_ChildClassHasPredecessor()
        {
            Assert.AreEqual(
                "Company",
                _analyzed.HasClassNamed("Customer__employer").HasPredecessorNamed("value").FactType);
        }

        [TestMethod]
        public void ChildClassHasIsCurrentPredicate()
        {
            Assert.AreEqual(
                "Customer__name",
                _analyzed.HasClassNamed("Customer__name").HasPredicateNamed("isCurrent").Query.Joins.Single().Type);
        }

        [TestMethod]
        public void ParentClassHasQuery()
        {
            Target.Join join = _analyzed.HasClassNamed("Customer").HasQueryNamed("name").Joins.Single();
            Assert.AreEqual("Customer__name", join.Type);
            Assert.AreEqual("isCurrent", join.Conditions.Single().Name);
            Assert.AreEqual("Customer__name", join.Conditions.Single().Type);
        }

        [TestMethod]
        public void ParentClassHasDisputableResultNative()
        {
            Target.Result result = _analyzed.HasClassNamed("Customer").HasResultNamed("name");
            Assert.AreEqual("Customer__name", result.Type);
            Target.ResultValueNative nativeDisputableResult = result as Target.ResultValueNative;
            Assert.IsNotNull(nativeDisputableResult);
            Assert.AreEqual(Target.NativeType.String, nativeDisputableResult.NativeType);
        }

        [TestMethod]
        public void ParentClassHasDisputableResultFact()
        {
            Target.Result result = _analyzed.HasClassNamed("Customer").HasResultNamed("employer");
            Assert.AreEqual("Customer__employer", result.Type);
            Target.ResultValueFact nativeDisputableResult = result as Target.ResultValueFact;
            Assert.IsNotNull(nativeDisputableResult);
            Assert.AreEqual("Company", nativeDisputableResult.FactType);
        }
    }
}
