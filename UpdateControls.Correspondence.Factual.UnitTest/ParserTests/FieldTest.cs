using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class FieldTest : TestBase
    {
        [TestMethod]
        public void WhenFieldIsPredecessor_PredecessorIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  GameQueue gameQueue;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("gameQueue")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("GameQueue")) &
                            Has<DataTypeFact>.Property(type => type.IsPivot, Is.EqualTo(false))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsPivot_PivotIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameRequest {\r\n" +
                "  publish GameQueue gameQueue;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("gameQueue")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.One)) &
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("GameQueue")) &
                            Has<DataTypeFact>.Property(type => type.IsPivot, Is.EqualTo(true))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenListIsPivot_PivotIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Game {\r\n" +
                "  publish User *players;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<DataMember>(), Contains<DataMember>.That(
                    Has<DataMember>.Property(field => field.Name, Is.EqualTo("players")) &
                    Has<DataMember>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.Many)) &
                        KindOf<DataType, DataTypeFact>.That(
                            Has<DataTypeFact>.Property(type => type.FactName, Is.EqualTo("User")) &
                            Has<DataTypeFact>.Property(type => type.IsPivot, Is.EqualTo(true))
                        )
                    ) &
                    Has<DataMember>.Property(field => field.LineNumber, Is.EqualTo(4))
                ))
            ));
        }

        [TestMethod]
        public void WhenFieldIsOptional_CardinalityIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact GameQueue {\r\n" +
                "  string? identifier;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members.OfType<Field>(), Contains<Field>.That(
                    Has<Field>.Property(field => field.Type,
                        Has<DataType>.Property(type => type.Cardinality, Is.EqualTo(Cardinality.Optional)))
                ))
            ));
        }
    }
}
