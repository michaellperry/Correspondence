using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class AsymmetricSecurityTest : TestBase
    {
        [TestMethod]
        public void WhenIdentity_IdentityIsRecognizes()
        {
            Namespace result = ParseToNamespace(
                "namespace IM.Model;" +
                "fact User {        " +
                "    identity;      " +
                "}                  ");
            Fact user = result.WithFactNamed("User");
            Assert.IsTrue(user.Identity);
        }

        [TestMethod]
        public void WhenUnmodified_FieldIsNotSecured()
        {
            string source =
                "namespace IM.Model; " +
                "fact Message {      " +
                "    Tag tag;        " +
                "}                   ";
            Namespace ns = ParseToNamespace(source);
            //ns2 = new NamespaceBuilder("IM.model").withFact("Message").withTag("tag").build();
            //AsserDeepEquals(ns, ns2);
            AssertNoSecurityModifier(ns.WithFactNamed("Message").WithFieldNamed("tag"));
        }

        [TestMethod]
        public void WhenTo_ToIsRecognized()
        {
            Namespace result = ParseToNamespace(
                "namespace IM.Model;    " +
                "fact Message {         " +
                "    to User recipient; " +
                "}                      ");
            Fact message = result.WithFactNamed("Message");
            Field recipient = message.WithFieldNamed("recipient");
            Assert.AreEqual(FieldSecurityModifier.To, recipient.SecurityModifier);
        }

        [TestMethod]
        public void WhenFrom_FromIsRecognized()
        {
            Namespace result = ParseToNamespace(
                "namespace IM.Model;   " +
                "fact Message {        " +
                "    from User sender; " +
                "}                     ");
            Fact message = result.WithFactNamed("Message");
            Field sender = message.WithFieldNamed("sender");
            Assert.AreEqual(FieldSecurityModifier.From, sender.SecurityModifier);
        }

        private static void AssertNoSecurityModifier(Field field)
        {
            Assert.AreEqual(FieldSecurityModifier.None, field.SecurityModifier);
        }
    }
}
