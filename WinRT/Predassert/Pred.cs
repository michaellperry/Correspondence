using System;

namespace Predassert
{
    public static class Pred
    {
        public static void Assert<T>(T actual, Expectation<T> expected)
        {
            if (!expected.Test(actual))
                Microsoft.VisualStudio.TestPlatform.UnitTestFramework.Assert.Fail(
                    expected.ReasonFalse(actual));
        }
    }
}
