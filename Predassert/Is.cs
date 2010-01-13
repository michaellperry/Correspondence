using System;
using System.Collections.Generic;
using System.Linq;

namespace Predassert
{
    public static class Is
    {
        public static Expectation<bool> True()
        {
            return new Expectation<bool>(
                actual => actual,
                actual => "The value is false.",
                actual => "The value is true."
            );
        }

        public static Expectation<bool> False()
        {
            return new Expectation<bool>(
                actual => !actual,
                actual => "The value is true.",
                actual => "The value is false."
            );
        }

        public static Expectation<IEnumerable<T>> Empty<T>()
        {
            return new Expectation<IEnumerable<T>>(
                actual => !actual.Any(),
                actual => string.Format("The collection should be empty, but instead contains {0} elements.", actual.Count()),
                actual => "The collection is empty."
            );
        }

        public static Expectation<T> EqualTo<T>(T expected)
        {
            return new Expectation<T>(
                actual => Object.Equals(actual, expected),
                actual => string.Format("Expected {0}, actual {1}.", expected, actual),
                actual => string.Format("It is equal to {0}.", expected)
            );
        }

        public static Expectation<T> SameAs<T>(T expected)
        {
            return new Expectation<T>(
                actual => Object.ReferenceEquals(actual, expected),
                actual => string.Format("{0} is not the same as {1}.", actual, expected),
                actual => string.Format("It is the same as {0}.", expected)
            );
        }

        public static Expectation<T> NotSameAs<T>(T expected)
        {
            return new Expectation<T>(
                actual => !Object.ReferenceEquals(actual, expected),
                actual => string.Format("It is the same as {0}.", expected),
                actual => string.Format("{0} is not the same as {1}.", actual, expected)
            );
        }

        public static Expectation<T> Null<T>()
        {
            return new Expectation<T>(
                actual => actual == null,
                actual => string.Format("The object should be null, but it is {0}.", actual),
                actual => "The object is null."
            );
        }

        public static Expectation<T> NotNull<T>()
        {
            return new Expectation<T>(
                actual => actual != null,
                actual => "The object is null.",
                actual => "The object is not null."
            );
        }
    }
}
