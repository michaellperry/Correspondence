﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests;
using UpdateControls.Correspondence.Factual.UnitTest.Properties;

namespace UpdateControls.Correspondence.Factual.UnitTest.AcceptanceTests
{
    [TestClass]
    public class AsymmetricSecurityAcceptanceTest
    {
        [TestMethod]
        public void CanParseSecureModel()
        {
            var code = Resources.AsymmetricSecurityModel;
            var parser = new FactualParser(new StringReader(code));
            var root = parser.Parse();
            if (root == null)
            {
                var error = parser.Errors.First();
                Assert.Fail(String.Format("{0}: {1}", error.LineNumber, error.Message));
            }
            root.AnalyzedHasNoError();
        }
    }
}
