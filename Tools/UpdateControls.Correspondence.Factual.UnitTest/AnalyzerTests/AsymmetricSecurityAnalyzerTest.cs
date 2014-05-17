using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;
using UpdateControls.Correspondence.Factual.Metadata;
using AST = UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    [TestClass]
    public class AsymmetricSecurityAnalyzerTest
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

        [TestMethod]
        public void WhenFrom_HasSignedByQuery()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 3))
                .AddFact(new Fact("Message", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "sender"))
                    .SetFromPath(AbsolutePath()
                        .AddSegment("sender")
                    )
                );

            var message = root
                .AnalyzedHasNoError()
                .HasClassNamed("Message");
            Metadata.Join signedBy = message.SignedBy;
            Assert.IsNotNull(signedBy, "The signer shold be set.");
            Assert.AreEqual(Direction.Predecessors, signedBy.Direction);
            Assert.IsFalse(signedBy.Conditions.Any(), "The signer should be unconditional.");
            Assert.AreEqual("sender", signedBy.Name);
            Assert.AreEqual("Individual", signedBy.Type);
        }

        [TestMethod]
        public void WhenTo_HasEncryptedForQuery()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Message", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "recipient"))
                    .SetFromPath(AbsolutePath()
                        .AddSegment("recipient")
                    )
                );

            Assert.Fail();
        }

        private static Namespace SecureNamespace()
        {
            return new Namespace("IM.Model", 1, new List<Header>(), "us_1_1");
        }

        private static DataTypeFact One(string factName)
        {
            return new DataTypeFact(factName, AST.Cardinality.One, 6);
        }

        private static AST.Path AbsolutePath()
        {
            return new AST.Path(absolute: true, relativeTo: null);
        }

        private static AST.Field PredecessorField(DataType dataType, string name)
        {
            return new AST.Field(6, name, dataType, false, null);
        }
    }
}
