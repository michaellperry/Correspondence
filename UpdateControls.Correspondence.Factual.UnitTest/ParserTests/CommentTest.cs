using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class CommentTest : TestBase
    {
        [TestMethod]
        public void WhenMultiLineCommentFound_CommentIsIgnored()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "/*fact Ignored\r\n" +
                "{\r\n" +
                "}\r\n" +
                "*/\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, ContainsNo<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Ignored"))
            ));
        }

        [TestMethod]
        public void WhenSingleLineCommentFound_CommentIsIgnored()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "  //string ignored;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, ContainsNo<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("ignored"))
                ))
            ));
        }
    }
}
