using Correspondence.Factual.AST;
using Correspondence.Factual.Metadata;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using Target = Correspondence.Factual.Metadata;
using Correspondence.Factual.Compiler;
using System.Text;


namespace Correspondence.Factual.UnitTest.AnalyzerTests
{
    public static class AnalyzerTestExtensions
    {
        public static Fact SetPrincipal(this Fact fact, bool value)
        {
            fact.Principal = value;
            return fact;
        }

        public static Fact SetLock(this Fact fact, bool value)
        {
            fact.Lock = value;
            return fact;
        }

        public static Fact SetUnique(this Fact fact, bool value)
        {
            fact.Unique = value;
            return fact;
        }

        public static void AnalyzedHasError(this Namespace root, int lineNumber, string message)
        {
            Analyzer analyzer = new Analyzer(root);
            Analyzed analyzed = analyzer.Analyze();
            Assert.IsNull(analyzed, "The analyzer should have raised an error.");
            Assert.AreEqual(1, analyzer.Errors.Count);

            var error = analyzer.Errors.Single();
            Assert.AreEqual(message, error.Message);
            Assert.AreEqual(lineNumber, error.LineNumber);
        }

        public static Analyzed AnalyzedHasNoError(this Namespace root)
        {
            Analyzer analyzer = new Analyzer(root);
            Analyzed analyzed = analyzer.Analyze();
            if (analyzed == null)
            {
                var error = analyzer.Errors.First();
                Assert.Fail(String.Format("{0}: {1}", error.LineNumber, error.Message));
            }
            return analyzed;
        }

        public static Class HasClassNamed(this Analyzed result, string name)
        {
            Assert.IsNotNull(result, "The analyzer returned an error.");
            IEnumerable<Class> classes = result.Classes.Where(c => c.Name == name);
            if (!classes.Any())
                Assert.Fail(String.Format("No class named {0} was found.", name));
            if (classes.Count() > 1)
                Assert.Fail(String.Format("More than one class named {0} was found.", name));
            Class user = classes.Single();
            return user;
        }

        public static Target.Predecessor HasPredecessorNamed(this Class c, string name)
        {
            IEnumerable<Target.Predecessor> predecessors = c.Predecessors.Where(predecessor => predecessor.Name == name);
            if (!predecessors.Any())
                Assert.Fail(String.Format("No predecessor named {0} was found.", name));
            if (predecessors.Count() > 1)
                Assert.Fail(String.Format("More than one predecessor named {0} was found.", name));
            return predecessors.Single();
        }

        public static Target.Field HasFieldNamed(this Class c, string name)
        {
            IEnumerable<Target.Field> fields = c.Fields.Where(field => field.Name == name);
            if (!fields.Any())
                Assert.Fail(String.Format("No field named {0} was found.", name));
            if (fields.Count() > 1)
                Assert.Fail(String.Format("More than one field named {0} was found.", name));
            return fields.Single();
        }

        public static Target.Predicate HasPredicateNamed(this Target.Class c, string name)
        {
            IEnumerable<Target.Predicate> predicates = c.Predicates.Where(predicate => predicate.Query.Name == name);
            if (!predicates.Any())
                Assert.Fail(String.Format("No predicate named {0} was found.", name));
            if (predicates.Count() > 1)
                Assert.Fail(String.Format("More than one predicate named {0} was found.", name));
            return predicates.Single();
        }

        public static Target.Query HasQueryNamed(this Target.Class c, string name)
        {
            IEnumerable<Target.Query> queries = c.Queries.Where(query => query.Name == name);
            if (!queries.Any())
                Assert.Fail(String.Format("No query named {0} was found.", name));
            if (queries.Count() > 1)
                Assert.Fail(String.Format("More than one query named {0} was found.", name));
            return queries.Single();
        }

        public static Target.Result HasResultNamed(this Target.Class c, string name)
        {
            IEnumerable<Target.Result> results = c.Results.Where(result => result.Query.Name == name);
            if (!results.Any())
                Assert.Fail(String.Format("No results named {0} was found.", name));
            if (results.Count() > 1)
                Assert.Fail(String.Format("More than one result named {0} was found.", name));
            return results.Single();
        }
    }
}
