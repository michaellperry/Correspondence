using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Namespace result = ParseToNamespace(source);
            Field tag = result.WithFactNamed("Message").WithFieldNamed("tag");
            Assert.AreEqual(FieldSecurityModifier.None, tag.SecurityModifier);
        }

        [TestMethod]
        public void WhenTo_ToIsRecognized()
        {
            string code =
                "namespace IM.Model;    " +
                "fact Message {         " +
                "    to User recipient; " +
                "}                      ";
            Namespace result = ParseToNamespace(code);
            Field recipient = result.WithFactNamed("Message").WithFieldNamed("recipient");
            Assert.AreEqual(FieldSecurityModifier.To, recipient.SecurityModifier);
        }

        [TestMethod]
        public void WhenFrom_FromIsRecognized()
        {
            string code =
                "namespace IM.Model;   " +
                "fact Message {        " +
                "    from User sender; " +
                "}                     ";
            Namespace result = ParseToNamespace(code);
            Field sender = result.WithFactNamed("Message").WithFieldNamed("sender");
            Assert.AreEqual(FieldSecurityModifier.From, sender.SecurityModifier);
        }
    }
}
