using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class CommentTest : TestBase
    {
        [TestMethod]
        public void WhenMultiLineCommentFound_CommentIsIgnored()
        {
            string code =
                "namespace ContactList;                      \r\n" +
                "                                            \r\n" +
                "/*fact Ignored                              \r\n" +
                "{                                           \r\n" +
                "}                                           \r\n" +
                "*/                                          \r\n" +
                "fact Person {                               \r\n" +
                "query:                                      \r\n" +
                "  Address* addresses {                      \r\n" +
                "    Address address : address.person = this \r\n" +
                "  }                                         \r\n" +
                "}";
            Namespace result = ParseToNamespace(code);
            Assert.IsFalse(result.Facts.Any(fact => fact.Name == "Ignored"), "The commented out fact was found.");
        }

        [TestMethod]
        public void WhenSingleLineCommentFound_CommentIsIgnored()
        {
            string code =
                "namespace ContactList;                      \r\n" +
                "                                            \r\n" +
                "fact Person {                               \r\n" +
                "query:                                      \r\n" +
                "  Address* addresses {                      \r\n" +
                "    Address address : address.person = this \r\n" +
                "  }                                         \r\n" +
                "  //string ignored;                         \r\n" +
                "}";
            Namespace result = ParseToNamespace(code);
            var person = result.WithFactNamed("Person");
            Assert.IsFalse(person.Members.Any(member => member.Name == "ignored"), "The commented out field was found.");
        }
    }
}
