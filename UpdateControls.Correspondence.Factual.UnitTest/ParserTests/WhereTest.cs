using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class WhereTest : TestBase
    {
        [TestMethod]
        public void WhenWhere_ConditionHasClause()
        {
            Namespace result = ParseToNamespace(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "	Queue queue;\r\n" +
                "	Time timestamp;\r\n" +
                "	\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted /*and not request.isCanceled*/\r\n" +
                "	}\r\n" +
                "}");
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("outstandingRequests")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Query>.That(
                        Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("request")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Request")) &
                            Has<Set>.Property(set => set.Condition,
                                Has<Condition>.Property(condition => condition.Clauses, Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isAccepted"))
                                ))
                            )
                        ))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenWhereAndAnd_ConditionHasTwoClauses()
        {
            Namespace result = ParseToNamespace(
                "namespace Reversi.GameModel;\r\n" +
                "\r\n" +
                "fact Frame {\r\n" +
                "	Queue queue;\r\n" +
                "	Time timestamp;\r\n" +
                "	\r\n" +
                "	Request* outstandingRequests {\r\n" +
                "		Request request : request.frame = this\r\n" +
                "			where not request.isAccepted and not request.isCanceled\r\n" +
                "	}\r\n" +
                "}"
            );
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    Has<FactMember>.Property(member => member.Name, Is.EqualTo("outstandingRequests")) &
                    Has<FactMember>.Property(member => member.LineNumber, Is.EqualTo(7)) &
                    KindOf<FactMember, Query>.That(
                        Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                            Has<Set>.Property(set => set.Name, Is.EqualTo("request")) &
                            Has<Set>.Property(set => set.FactName, Is.EqualTo("Request")) &
                            Has<Set>.Property(set => set.Condition,
                                Has<Condition>.Property(condition => condition.Clauses, Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isAccepted"))
                                ) & Contains<Clause>.That(
                                    Has<Clause>.Property(clause => clause.Existence, Is.EqualTo(ConditionModifier.Negative)) &
                                    Has<Clause>.Property(clause => clause.Name, Is.EqualTo("request")) &
                                    Has<Clause>.Property(clause => clause.PredicateName, Is.EqualTo("isCanceled"))
                                ))
                            )
                        ))
                    )
                ))
            ));
        }
    }
}
