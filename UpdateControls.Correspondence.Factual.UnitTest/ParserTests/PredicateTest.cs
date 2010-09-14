using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class PredicateTest : TestBase
    {
        [TestMethod]
        public void WhenPredicateIsNegative_ExistenceIsNegative()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Request {\r\n" +
                "	Frame frame;\r\n" +
                "	Person requester;\r\n" +
                "	\r\n" +
                "	bool isOutstanding {\r\n" +
                "		not exists Accept accept : accept.request = this\r\n" +
                "	}\r\n" +
                "	\r\n" +
                "	Bid* bids {\r\n" +
                "		Bid bid : bid.request = this\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("isOutstanding")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Predicate>.That(
                        Has<Predicate>.Property(predicate => predicate.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                        Has<Predicate>.Property(predicate => predicate.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("accept")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Accept"))
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenPredicateIsPositive_ExistenceIsPositive()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "	bool isPlaying {\r\n" +
                "		exists Game game : game.player.person = this\r\n" +
                "	}\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("isPlaying")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(4)) &
                    KindOf<FactMember, Predicate>.That(
                        Has<Predicate>.Property(predicate => predicate.Existence, Is.EqualTo(ConditionModifier.Positive)) &
                        Has<Predicate>.Property(predicate => predicate.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("game")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Game"))
                        ))
                    )
                ))
            ));
        }
    }
}
