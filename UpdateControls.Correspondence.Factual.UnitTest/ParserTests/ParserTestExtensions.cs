using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateControls.Correspondence.Factual.AST;

namespace UpdateControls.Correspondence.Factual.UnitTest.ParserTests
{
    public static class ParserTestExtensions
    {
        public static Fact WithFactNamed(this Namespace result, string name)
        {
            var facts = result.Facts.Where(fact => fact.Name == name);
            if (!facts.Any())
                Assert.Fail(String.Format("No fact named {0} was found.", name));
            if (facts.Count() > 1)
                Assert.Fail(String.Format("More than one fact is named {0}.", name));
            return facts.Single();
        }

        public static Field WithFieldNamed(this Fact fact, string name)
        {
            var fields = fact.Members.OfType<Field>().Where(field => field.Name == name);
            if (!fields.Any())
                Assert.Fail(string.Format("The fact {0} contains no field named {1}.", fact.Name, name));
            if (fields.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one field named {1}.", fact.Name, name));
            return fields.Single();
        }

        public static Query WithQueryNamed(this Fact fact, string name)
        {
            var queries = fact.Members.OfType<Query>().Where(field => field.Name == name);
            if (!queries.Any())
                Assert.Fail(string.Format("The fact {0} contains no query named {1}.", fact.Name, name));
            if (queries.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one query named {1}.", fact.Name, name));
            return queries.Single();
        }

        public static DataTypeFact ThatIsDataTypeFact(this DataType dataType)
        {
            Assert.IsInstanceOfType(dataType, typeof(DataTypeFact));
            return (DataTypeFact)dataType;
        }

        public static DataTypeNative ThatIsDataTypeNative(this DataType dataType)
        {
            Assert.IsInstanceOfType(dataType, typeof(DataTypeNative));
            return (DataTypeNative)dataType;
        }
    }
}
