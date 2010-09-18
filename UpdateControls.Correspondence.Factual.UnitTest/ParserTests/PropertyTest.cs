using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
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
                "  property string firstName; " +
                "}                            ";
            Namespace result = ParseToNamespace(code);
            DataTypeNative dataTypeNative = result.WithFactNamed("Person").WithPropertyNamed("firstName")
                .Type.ThatIsDataTypeNative();
            Assert.AreEqual(NativeType.String, dataTypeNative.NativeType);
        }
    }
}
