﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.Compiler;
using Source = UpdateControls.Correspondence.Factual.AST;
using Target = UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class MutableTest
    {
        private Target.Analyzed _analyzed;

        [TestInitialize]
        public void Initialize()
        {
            Source.Namespace root = new Source.Namespace("CRM.Model", 1, string.Empty)
                .AddFact(new Source.Fact("Customer", 3)
                    .AddMember(new Source.Property(5, "name", new Source.DataTypeNative(Source.NativeType.String, Source.Cardinality.One, 5)))
                    .AddMember(new Source.Property(6, "employer", new Source.DataTypeFact("Company", Source.Cardinality.One, false, 6)))
                );

            _analyzed = new Analyzer(root).Analyze();
        }

        [TestMethod]
        public void MutablePropertyCreatesAChildClass()
        {
            _analyzed.HasClassNamed("CustomerName");
        }

        [TestMethod]
        public void ChildClassHasParentPredecessor()
        {
            Assert.AreEqual(
                "Customer",
                _analyzed.HasClassNamed("CustomerName").HasPredecessorNamed("customer").FactType);
        }

        [TestMethod]
        public void ChildClassHasPriorPredecessor()
        {
            Assert.AreEqual(
                "CustomerName",
                _analyzed.HasClassNamed("CustomerName").HasPredecessorNamed("prior").FactType);
        }

        [TestMethod]
        public void ChildClassHasValueField()
        {
            Assert.AreEqual(
                Target.NativeType.String,
                _analyzed.HasClassNamed("CustomerName").HasFieldNamed("value").NativeType);
        }

        [TestMethod]
        public void WhenMutableTypeIsNotNative_ChildClassHasPredecessor()
        {
            Assert.AreEqual(
                "Company",
                _analyzed.HasClassNamed("CustomerEmployer").HasPredecessorNamed("value").FactType);
        }

        [TestMethod]
        public void ChildClassHasIsCurrentPredicate()
        {
            Assert.AreEqual(
                "CustomerName",
                _analyzed.HasClassNamed("CustomerName").HasPredicateNamed("isCurrent").Query.Joins.Single().Type);
        }
        
    }
}
