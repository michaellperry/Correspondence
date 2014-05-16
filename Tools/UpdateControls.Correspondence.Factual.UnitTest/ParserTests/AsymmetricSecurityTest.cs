using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class AsymmetricSecurityTest : TestBase
    {
        [TestMethod]
        public void WhenNoStrength_StrenthIsEmpty()
        {
            Namespace result = AssertNoErrors(
                "namespace IM.Model;");

            Assert.AreEqual(string.Empty, result.Strength);
        }

        [TestMethod]
        public void WhenStrength_StrenthIsRecognized()
        {
            Namespace result = AssertNoErrors(
                "namespace IM.Model;\r\n" +
                "strength us_1_0;");

            Assert.AreEqual("us_1_0", result.Strength);
        }

        [TestMethod]
        public void WhenIdentity_IdentityIsRecognizes()
        {
            Namespace result = ParseToNamespace(
                "namespace IM.Model;" +
                "fact User {        " +
                "key:               " +
                "    principal;     " +
                "}                  ");
            Fact user = result.WithFactNamed("User");
            Assert.IsTrue(user.Principal);
        }

        [TestMethod]
        public void WhenUnmodified_FieldIsNotSecured()
        {
            string source =
                "namespace IM.Model; " +
                "fact Message {      " +
                "key:               " +
                "    Tag tag;        " +
                "}                   ";
            Namespace result = ParseToNamespace(source);
            Fact message = result.WithFactNamed("Message");
            Assert.IsNull(message.ToPath);
        }

        [TestMethod]
        public void WhenTo_ToIsRecognized()
        {
            string code =
                "namespace IM.Model;    " +
                "fact Message {         " +
                "key:                   " +
                "    User recipient;    " +
                "    to recipient;      " +
                "}                      ";
            Namespace result = ParseToNamespace(code);
            Fact message = result.WithFactNamed("Message");
            Path path = message.ToPath;
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Absolute, "The collaborator path should be absolute.");
            Assert.AreEqual(1, path.Segments.Count());
            string segment = path.Segments.Single();
            Assert.AreEqual("recipient", segment);
        }

        [TestMethod]
        public void WhenFrom_FromIsRecognized()
        {
            string code =
                "namespace IM.Model;   " +
                "fact Message {        " +
                "key:                  " +
                "    User sender;      " +
                "    from sender;      " +
                "}                     ";
            Namespace result = ParseToNamespace(code);
            Fact message = result.WithFactNamed("Message");
            Field sender = message.WithFieldNamed("sender");
            Path fromPath = message.FromPath;
            Assert.IsTrue(fromPath.Absolute, "The collaborator path should be absolute.");
            Assert.AreEqual(1, fromPath.Segments.Count());
            Assert.AreEqual("sender", fromPath.Segments.Single());
        }

        [TestMethod]
        public void WhenLock_LockIsRecognized()
        {
            string code =
                "namespace IM.Model;      " +
                "fact PrivateBoard {      " +
                "key:                     " +
                "    unique;              " +
                "    lock;                " +
                "}                        ";
            Namespace result = AssertNoErrors(code);

            var fact = result.WithFactNamed("PrivateBoard");
            Assert.IsTrue(fact.Lock);
        }
    }
}
