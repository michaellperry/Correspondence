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
                    .SetFromPath(AbsolutePath(6)
                        .AddSegment("sender")
                    )
                );

            var message = root
                .AnalyzedHasNoError()
                .HasClassNamed("Message");
            Metadata.Path signedBy = message.SignedBy;
            Assert.IsNotNull(signedBy, "The signer should be set.");
            IEnumerable<Metadata.Segment> segments = signedBy.Segments;
            Assert.AreEqual(1, segments.Count());
            var segment = segments.Single();
            Assert.AreEqual("sender", segment.Name);
            Assert.AreEqual("Individual", segment.Type);
        }

        [TestMethod]
        public void WhenFromNotAMember_Error()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 3))
                .AddFact(new Fact("Message", 4)
                    .SetFromPath(AbsolutePath(6)
                        .AddSegment("sender")
                    )
                );

            root.AnalyzedHasError(6, "The member \"Message.sender\" is not defined.");
        }

        [TestMethod]
        public void WhenFromNotAPredecessor_Error()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 3))
                .AddFact(new Fact("Message", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "sentBy"))
                    .SetFromPath(AbsolutePath(6)
                        .AddSegment("sender")
                    )
                    .AddMember(new AST.Query("sender", "Individual", 8)
                        .AddSet(new AST.Set("i", "Individual",
                            new AST.Path(true, null, 9)
                                .AddSegment("sentBy"),
                            new AST.Path(false, "i", 10),
                            null, 9)
                        )
                    )
                );

            root.AnalyzedHasError(6, "The member \"Message.sender\" is not a field.");
        }

        [TestMethod]
        public void WhenFromMany_Error()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 3))
                .AddFact(new Fact("Message", 4)
                    .AddMember(PredecessorField(
                        Many("Individual"),
                        "sender"))
                    .SetFromPath(AbsolutePath(6)
                        .AddSegment("sender")
                    )
                );

            root.AnalyzedHasError(6, "The member \"Message.sender\" is not a single fact.");
        }

        [TestMethod]
        public void WhenTo_HasEncryptedForQuery()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 3))
                .AddFact(new Fact("Message", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "recipient"))
                    .SetToPath(AbsolutePath(6)
                        .AddSegment("recipient")
                    )
                );

            var message = root
                .AnalyzedHasNoError()
                .HasClassNamed("Message");
            Metadata.Path encryptedFor = message.EncryptedFor;
            Assert.IsNotNull(encryptedFor, "The encrypted for path should be set.");
            IEnumerable<Metadata.Segment> segments = encryptedFor.Segments;
            Assert.AreEqual(1, segments.Count());
            var segment = segments.Single();
            Assert.AreEqual("recipient", segment.Name);
            Assert.AreEqual("Individual", segment.Type);
        }

        [TestMethod]
        public void WhenUnlock_HasKeyForQuery()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 2))
                .AddFact(new Fact("MessageBoard", 3)
                    .SetUnique(true)
                    .SetLock(true)
                )
                .AddFact(new Fact("Membership", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "recipient"))
                    .AddMember(PredecessorField(
                        One("MessageBoard"),
                        "messageBoard"))
                    .SetToPath(AbsolutePath(5)
                        .AddSegment("recipient"))
                    .SetUnlockPath(AbsolutePath(6)
                        .AddSegment("messageBoard"))
                );

            var membership = root.AnalyzedHasNoError()
                .HasClassNamed("Membership");
            Metadata.Path keyFor = membership.KeyFor;
            Assert.IsNotNull(keyFor, "The key for path should be set.");
            var segments = keyFor.Segments;
            Assert.AreEqual(1, segments.Count());
            var segment = segments.Single();
            Assert.AreEqual("messageBoard", segment.Name);
            Assert.AreEqual("MessageBoard", segment.Type);
        }

        [TestMethod]
        public void WhenUnlockIsNotLocked_Error()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 2))
                .AddFact(new Fact("MessageBoard", 3)
                    .SetUnique(true)
                )
                .AddFact(new Fact("Membership", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "recipient"))
                    .AddMember(PredecessorField(
                        One("MessageBoard"),
                        "messageBoard"))
                    .SetToPath(AbsolutePath(5)
                        .AddSegment("recipient"))
                    .SetUnlockPath(AbsolutePath(6)
                        .AddSegment("messageBoard"))
                );

            root.AnalyzedHasError(6, "The fact \"MessageBoard\" is not locked.");
        }

        [TestMethod]
        public void WhenUnlockedAndNotEncrypted_Error()
        {
            Namespace root = SecureNamespace()
                .AddFact(new Fact("Individual", 2))
                .AddFact(new Fact("MessageBoard", 3)
                    .SetUnique(true)
                    .SetLock(true)
                )
                .AddFact(new Fact("Membership", 4)
                    .AddMember(PredecessorField(
                        One("Individual"),
                        "recipient"))
                    .AddMember(PredecessorField(
                        One("MessageBoard"),
                        "messageBoard"))
                    .SetUnlockPath(AbsolutePath(6)
                        .AddSegment("messageBoard"))
                );

            root.AnalyzedHasError(6, "The fact \"Membership\" is not encrypted.");
        }

        private static Namespace SecureNamespace()
        {
            return new Namespace("IM.Model", 1, new List<Header>(), "us_1_1");
        }

        private static DataTypeFact One(string factName)
        {
            return new DataTypeFact(factName, AST.Cardinality.One, 6);
        }

        private static DataTypeFact Many(string factName)
        {
            return new DataTypeFact(factName, AST.Cardinality.Many, 6);
        }

        private static AST.Path AbsolutePath(int lineNumber)
        {
            return new AST.Path(absolute: true, relativeTo: null, lineNumber: lineNumber);
        }

        private static AST.Field PredecessorField(DataType dataType, string name)
        {
            return new AST.Field(6, name, dataType, false, null);
        }
    }
}
