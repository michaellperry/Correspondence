using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Predassert;
using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Compiler;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    [TestClass]
    public class AsymmetricSecurityTest : TestBase
    {
        [TestMethod]
        public void WhenIdentity_IdentityIsRecognizes()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace IM.Model;\r\n" +
                "\r\n" +
                "fact User {\r\n" +
                "    unique;\r\n" +
                "    identity;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("User")) &
                Has<Fact>.Property(fact => fact.Identity, Is.EqualTo(true))
            ));
        }

        [TestMethod]
        public void WhenUnmodified_FieldIsNotSecured()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace IM.Model;\r\n" +
                "\r\n" +
                "fact Tag {\r\n" +
                "    string name;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Message {\r\n" +
                "    string body;\r\n" +
                "    Tag tag;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Message")) &
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    KindOf<FactMember, Field>.That(
                        Has<Field>.Property(field => field.Name, Is.EqualTo("tag")) &
                        Has<Field>.Property(field => field.SecurityModifier, Is.EqualTo(FieldSecurityModifier.None))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenTo_ToIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace IM.Model;\r\n" +
                "\r\n" +
                "fact Tag {\r\n" +
                "    string name;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact User {\r\n" +
                "    unique;\r\n" +
                "    identity;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Message {\r\n" +
                "    string body;\r\n" +
                "    Tag tag;\r\n" +
                "    to User recipient;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Message")) &
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    KindOf<FactMember, Field>.That(
                        Has<Field>.Property(field => field.Name, Is.EqualTo("recipient")) &
                        Has<Field>.Property(field => field.SecurityModifier, Is.EqualTo(FieldSecurityModifier.To))
                    )
                ))
            ));
        }

        [TestMethod]
        public void WhenFrom_FromIsRecognized()
        {
            FactualParser parser = new FactualParser(new StringReader(
                "namespace IM.Model;\r\n" +
                "\r\n" +
                "fact Tag {\r\n" +
                "    string name;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact User {\r\n" +
                "    unique;\r\n" +
                "    identity;\r\n" +
                "}\r\n" +
                "\r\n" +
                "fact Message {\r\n" +
                "    string body;\r\n" +
                "    Tag tag;\r\n" +
                "    to User recipient;\r\n" +
                "    from User sender;\r\n" +
                "}"
            ));
            Namespace result = AssertNoErrors(parser);
            Pred.Assert(result.Facts, Contains<Fact>.That(
                Has<Fact>.Property(fact => fact.Name, Is.EqualTo("Message")) &
                Has<Fact>.Property(fact => fact.Members, Contains<FactMember>.That(
                    KindOf<FactMember, Field>.That(
                        Has<Field>.Property(field => field.Name, Is.EqualTo("sender")) &
                        Has<Field>.Property(field => field.SecurityModifier, Is.EqualTo(FieldSecurityModifier.From))
                    )
                ))
            ));
        }
    }
}
