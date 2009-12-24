using System;
using System.Collections.Generic;
using System.Linq;

namespace Predassert
{
    public static class Is
    {
        public static Expectation<IEnumerable<T>> Empty<T>()
        {
            return new Expectation<IEnumerable<T>>(
                actual => !actual.Any(),
                actual => string.Format("The collection should be empty, but instead contains {0} elements.", actual.Count()));
        }

        public static Expectation<T> SameAs<T>(T other)
        {
            return new Expectation<T>(
                actual => Object.ReferenceEquals(actual, other),
                actual => string.Format("The objects should be the same, but {0} is not {1}.", actual, other));
        }

        public static Expectation<T> NotSameAs<T>(T other)
        {
            return new Expectation<T>(
                actual => !Object.ReferenceEquals(actual, other),
                actual => string.Format("The objects should not be the same, but {0} is {1}.", actual, other));
        }

        public static Expectation<T> Null<T>()
        {
            return new Expectation<T>(
                actual => actual == null,
                actual => string.Format("The object should be null, but it is {0}.", actual));
        }

        public static Expectation<T> NotNull<T>()
        {
            return new Expectation<T>(
                actual => actual != null,
                actual => "The object should not be null.");
        }
    }
}
