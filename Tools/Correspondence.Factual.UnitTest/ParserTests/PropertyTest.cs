using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.AST;

namespace Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class PropertyTest : TestBase
    {
        [TestMethod]
        public void WhenFactHasProperty_PropertyIsRecognized()
        {
            string code =
                "namespace ContactList;       " +
                "                             " +
                "fact Person {                " +
                "key:                         " +
                "mutable:                     " +
                "  string firstName;          " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            DataTypeNative dataTypeNative = result.WithFactNamed("Person").WithPropertyNamed("firstName")
                .Type.ThatIsDataTypeNative();
            Assert.AreEqual(NativeType.String, dataTypeNative.NativeType);
        }

        [TestMethod]
        public void CanPublishAProperty()
        {
            string code =
                "namespace ContactList;       " +
                "                             " +
                "fact Person {                " +
                "key:                         " +
                "mutable:                     " +
                "  publish string firstName;  " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            Property property = result.WithFactNamed("Person").WithPropertyNamed("firstName");

            Assert.IsTrue(property.Publish);
        }
    }
}
