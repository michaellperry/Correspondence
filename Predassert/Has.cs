using System;
using System.Linq.Expressions;

namespace Predassert
{
    public static class Has<T>
    {
        public static Expectation<T> Property<TProperty>(Expression<Func<T, TProperty>> property, Expectation<TProperty> expected)
        {
            return new Expectation<T>(
                actual => expected.Test(property.Compile()(actual)),
                actual => string.Format("The property {0} is not correct. {1}",
                    ((MemberExpression)property.Body).Member.Name,
                    expected.ReasonFalse(property.Compile()(actual))),
                actual => string.Format("The property {0} is correct. {1}",
                    ((MemberExpression)property.Body).Member.Name,
                    expected.ReasonTrue(property.Compile()(actual)))
                );
        }
    }
}
