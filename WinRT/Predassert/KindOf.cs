using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Predassert
{
    public static class KindOf<TBase, TDerived>
        where TDerived : class
    {
        public static Expectation<TBase> That(Expectation<TDerived> expected)
        {
            return new Expectation<TBase>(
                actual => actual is TDerived && expected.Test(actual as TDerived),
                actual => !(actual is TDerived) ? "The object is not a " + typeof(TDerived).Name : expected.ReasonFalse(actual as TDerived),
                actual => expected.ReasonTrue(actual as TDerived));
        }
    }
}
