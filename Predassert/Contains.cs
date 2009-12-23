using System;
using System.Collections.Generic;
using System.Linq;

namespace Predassert
{
    public static class Contains<T>
    {
        public static Expectation<IEnumerable<T>> That(Expectation<T> expected)
        {
            return new Expectation<IEnumerable<T>>(
                collection => collection.Any(expected.Test),
                collection => string.Format("The collection does not contain any matching elements.\r\n  {0}",
                    Reasons(collection, expected))
                );
        }

        private static string Reasons(IEnumerable<T> collection, Expectation<T> expected)
        {
            if (!collection.Any())
                return "The collection is empty.";
            else
                return string.Join("\r\n  ", collection.Select(actual => expected.Message(actual)).ToArray());
        }
    }
}
