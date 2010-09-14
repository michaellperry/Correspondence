using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class QueryTest : TestBase
    {
        [TestMethod]
        public void WhenFactHasQuery_QueryIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace ContactList;\r\n" +
                "\r\n" +
                "fact Person {\r\n" +
                "  Address* addresses {\r\n" +
                "    Address address : address.person = this\r\n" +
                "  }\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(KindOf<FactMember, Query>.That(
                    Has<Query>.Property(query => query.Name, Is.EqualTo("addresses")) &
                    Has<Query>.Property(query => query.FactName, Is.EqualTo("Address")) &
                    Has<Query>.Property(query => query.Sets, Contains<Set>.That(
                        Has<Set>.Property(set => set.Name, Is.EqualTo("address")) &
                        Has<Set>.Property(set => set.FactName, Is.EqualTo("Address")) &
                        Has<Set>.Property(set => set.LeftPath,
                            Has<AST.Path>.Property(path => path.Absolute, Is.EqualTo(false)) &
                            Has<AST.Path>.Property(path => path.RelativeTo, Is.EqualTo("address")) &
                            Has<AST.Path>.Property(path => path.Segments,
                                Contains<string>.That(Is.EqualTo("person"))
                            )
                        ) &
                        Has<Set>.Property(set => set.RightPath,
                            Has<AST.Path>.Property(path => path.Absolute, Is.EqualTo(true))
                        )
                    ))
                )))
            ));
        }
    }
}
