using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class AsymmetricSecurityAnalyzer : TestBase
    {
        [TestMethod]
        public void IdentityFactHasPublicKey()
        {
            Namespace root = new Namespace("IM.Model", 1)
                .AddFact(new Fact("User", 2)
                    .SetIdentity(true)
                );
            Analyzed analyzed = new Analyzer(root).Analyze();
            Class user = analyzed.HasClassNamed("User");
            Assert.IsTrue(user.HasPublicKey);
        }
    }
}
