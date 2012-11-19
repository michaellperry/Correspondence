using System;
using System.Collections.Generic;
using System.Linq;

namespace Predassert
{
    public static class ContainsNo<T>
    {
        public static Expectation<IEnumerable<T>> That(Expectation<T> expected)
        {
            return new Expectation<IEnumerable<T>>(
                collection => !collection.Any(expected.Test),
                collection => string.Format("The collection contains matching elements.\r\n  {0}",
                    ReasonsTrue(collection, expected)),
                collection => string.Format("The collection contains no matching element.\r\n  {0}",
                    ReasonsFalse(collection, expected))
                );
        }

        private static string ReasonsTrue(IEnumerable<T> collection, Expectation<T> expected)
        {
            return string.Join("\r\n  ", collection
                .Where(actual => expected.Test(actual))
                .Select(actual => actual.ToString())
                .ToArray());
        }

        private static string ReasonsFalse(IEnumerable<T> collection, Expectation<T> expected)
        {
            if (!collection.Any())
                return "The collection is empty.";
            else
                return string.Join("\r\n  ", collection
                    .Where(actual => !expected.Test(actual))
                    .Select(actual => expected.ReasonFalse(actual))
                    .ToArray());
        }
    }
}
