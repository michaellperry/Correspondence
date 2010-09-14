using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class PropertyTest : TestBase
    {
        [TestMethod]
        public void WhenFactHasProperty_PropertyIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  property string firstName;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Property>(), Contains<Property>.That(
                    Has<Property>.Property(property => property.Name, Is.EqualTo("firstName")) &
                    Has<Property>.Property(property => property.Type, KindOf<DataType, DataTypeNative>.That(
                        Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String))
                    ))
                ))
            ));
        }
    }
}
