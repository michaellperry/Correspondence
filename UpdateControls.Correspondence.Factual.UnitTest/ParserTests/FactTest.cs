using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class FactTest : TestBase
    {
        [TestMethod]
        public void WhenFactIsGiven_FactIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")) &
                Has<Fact>.Property(fact => fact.LineNumber, Is.EqualTo(3))
            ));
        }

        [TestMethod]
        public void WhenFactHasField_FieldIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string identifier;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("GameQueue")) &
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("identifier")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeNative>.That(
                            Has<DataTypeNative>.Property(type => type.NativeType, Is.EqualTo(NativeType.String))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }
    }
}
