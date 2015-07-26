using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Correspondence.Factual.AST;

namespace Correspondence.Factual.UnitTest.ParserTests
{
    public static class ParserTestExtensions
    {
        public static Fact WithFactNamed(this Namespace result, string name)
        {
            var facts = result.Facts.Where(fact => fact.Alias == name);
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
                Assert.Fail(string.Format("The fact {0} contains no field named {1}.", fact.Alias, name));
            if (fields.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one field named {1}.", fact.Alias, name));
            return fields.Single();
        }

        public static Query WithQueryNamed(this Fact fact, string name)
        {
            var queries = fact.Members.OfType<Query>().Where(field => field.Name == name);
            if (!queries.Any())
                Assert.Fail(string.Format("The fact {0} contains no query named {1}.", fact.Alias, name));
            if (queries.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one query named {1}.", fact.Alias, name));
            return queries.Single();
        }

        public static Predicate WithPredicateNamed(this Fact fact, string name)
        {
            var predicates = fact.Members.OfType<Predicate>().Where(field => field.Name == name);
            if (!predicates.Any())
                Assert.Fail(string.Format("The fact {0} contains no predicate named {1}.", fact.Alias, name));
            if (predicates.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one predicate named {1}.", fact.Alias, name));
            return predicates.Single();
        }

        public static Property WithPropertyNamed(this Fact fact, string name)
        {
            var properties = fact.Members.OfType<Property>().Where(field => field.Name == name);
            if (!properties.Any())
                Assert.Fail(string.Format("The fact {0} contains no property named {1}.", fact.Alias, name));
            if (properties.Count() > 1)
                Assert.Fail(String.Format("The fact {0} contains more than one property named {1}.", fact.Alias, name));
            return properties.Single();
        }

        public static Set WithSetNamed(this Query query, string name)
        {
            var sets = query.Sets.Where(set => set.Name == name);
            if (!sets.Any())
                Assert.Fail(string.Format("The query contains no set named {0}.", name));
            if (sets.Count() > 1)
                Assert.Fail(string.Format("The query contains more than one set named {0}.", name));
            return sets.Single();
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
