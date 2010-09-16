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
            Namespace result = AssertNoErrors(
                "namespace IM.Model;" +
                "fact User {        " +
                "    identity;      " +
                "}                  ");
            Fact user = result.AssertHasFactNamed("User");
            Assert.IsTrue(user.Identity);
        }

        [TestMethod]
        public void WhenUnmodified_FieldIsNotSecured()
        {
            Namespace result = AssertNoErrors(
                "namespace IM.Model; " +
                "fact Message {      " +
                "    Tag tag;        " +
                "}                   ");
            Fact message = result.AssertHasFactNamed("Message");
            Field tag = message.AssertHasFieldNamed("tag");
            Assert.AreEqual(FieldSecurityModifier.None, tag.SecurityModifier);
        }

        [TestMethod]
        public void WhenTo_ToIsRecognized()
        {
            Namespace result = AssertNoErrors(
                "namespace IM.Model;    " +
                "fact Message {         " +
                "    to User recipient; " +
                "}                      ");
            Fact message = result.AssertHasFactNamed("Message");
            Field recipient = message.AssertHasFieldNamed("recipient");
            Assert.AreEqual(FieldSecurityModifier.To, recipient.SecurityModifier);
        }

        [TestMethod]
        public void WhenFrom_FromIsRecognized()
        {
            Namespace result = AssertNoErrors(
                "namespace IM.Model;   " +
                "fact Message {        " +
                "    from User sender; " +
                "}                     ");
            Fact message = result.AssertHasFactNamed("Message");
            Field sender = message.AssertHasFieldNamed("sender");
            Assert.AreEqual(FieldSecurityModifier.From, sender.SecurityModifier);
        }
    }
}
