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
                collection => !collection.Any(),
                collection => string.Format("The collection should be empty, but instead contains {0} elements.", collection.Count()));
        }

        public static Expectation<T> SameAs<T>(T other)
        {
            return new Expectation<T>(
                obj => Object.ReferenceEquals(obj, other),
                obj => string.Format("The objects should be the same, but {0} is not {1}.", obj, other));
        }

        public static Expectation<T> NotSameAs<T>(T other)
        {
            return new Expectation<T>(
                obj => !Object.ReferenceEquals(obj, other),
                obj => string.Format("The objects should not be the same, but {0} is {1}.", obj, other));
        }
    }
}
