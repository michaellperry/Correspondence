﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.AST;
using Correspondence.Factual.Compiler;
using Correspondence.Factual.Metadata;

namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class AsymmetricSecurityAnalyzer
    {
        [TestMethod]
        public void WhenPrincipal_MustHaveAStrength()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), string.Empty)
                .AddFact(new Fact("User", 2)
                    .SetPrincipal(true)
                );
            root.AnalyzedHasError(2,
                "The fact \"User\" is a principal. The model must have a declared strength.");
        }

        [TestMethod]
        public void WhenPrincipal_MustBeUnique()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), "us_1_1")
                .AddFact(new Fact("User", 2)
                    .SetPrincipal(true)
                );
            root.AnalyzedHasError(2,
                "The fact \"User\" is a principal. It must also be unique.");
        }

        [TestMethod]
        public void WhenPrincipal_HasPublicKey()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), "us_1_1")
                .AddFact(new Fact("User", 2)
                    .SetPrincipal(true)
                    .SetUnique(true)
                );
            Class user = root
                .AnalyzedHasNoError()
                .HasClassNamed("User");
            Assert.IsTrue(user.HasPublicKey);
        }

        [TestMethod]
        public void WhenLock_MustBeUnique()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), "us_1_1")
                .AddFact(new Fact("PrivateBoard", 2)
                    .SetLock(true)
                );
            root.AnalyzedHasError(2,
                "The fact \"PrivateBoard\" is locked. It must also be unique.");
        }

        [TestMethod]
        public void WhenLock_CannotAlsoBePrincipal()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), "us_1_1")
                .AddFact(new Fact("PrivateBoard", 2)
                    .SetLock(true)
                    .SetUnique(true)
                    .SetPrincipal(true)
                );
            root.AnalyzedHasError(2,
                "The fact \"PrivateBoard\" is a principal. It cannot also be locked.");
        }

        [TestMethod]
        public void WhenLocked_HasSharedKey()
        {
            Namespace root = new Namespace("IM.Model", 1, new List<Header>(), "us_1_1")
                .AddFact(new Fact("PrivateBoard", 2)
                    .SetLock(true)
                    .SetUnique(true)
                );
            Class user = root
                .AnalyzedHasNoError()
                .HasClassNamed("PrivateBoard");
            Assert.IsTrue(user.HasSharedKey);
        }
    }
}
