using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class QueryTest : TestBase
    {
        [TestMethod]
        public void WhenFactHasQuery_QueryIsRecognized()
        {
            string code =
                "namespace ContactList;                      " +
                "                                            " +
                "fact Person {                               " +
                "key:                                        " +
                "query:                                      " +
                "  Address* addresses {                      " +
                "    Address address : address.person = this " +
                "  }                                         " +
                "}                                           ";
            Namespace result = ParseToNamespace(code);
            Query addresses = result.WithFactNamed("Person").WithQueryNamed("addresses");
            Assert.AreEqual("Address", addresses.FactName);
            Set set = addresses.Sets.Single();
            Assert.AreEqual("address", set.Name);
            Assert.AreEqual("Address", set.FactName);
            Assert.IsFalse(set.LeftPath.Absolute, "The left path is absolute.");
            Assert.AreEqual("address", set.LeftPath.RelativeTo);
            string segment = set.LeftPath.Segments.Single();
            Assert.AreEqual("person", segment);
            Assert.IsTrue(set.RightPath.Absolute, "The right path is not absolute.");
        }
    }
}
