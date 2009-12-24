using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
                    expected.Message(property.Compile()(actual)))
                );
        }
    }
}
