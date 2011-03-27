using UpdateControls.Correspondence.Factual.AST;
using UpdateControls.Correspondence.Factual.Metadata;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using Target = UpdateControls.Correspondence.Factual.Metadata;

namespace UpdateControls.Correspondence.Factual.UnitTest.AnalyzerTests
{
    public static class AnalyzerTestExtensions
    {
        public static Fact SetIdentity(this Fact fact, bool value)
        {
            fact.Principal = value;
            return fact;
        }

        public static Class HasClassNamed(this Analyzed result, string name)
        {
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
            IEnumerable<Target.Predecessor> predecessors = c.Predecessors.Where(field => field.Name == name);
            if (!predecessors.Any())
                Assert.Fail(String.Format("No predecessor named {0} was found.", name));
            if (predecessors.Count() > 1)
                Assert.Fail(String.Format("More than one predecessor named {0} was found.", name));
            return predecessors.Single();
        }
    }
}
