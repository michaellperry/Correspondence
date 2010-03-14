using System;

namespace Predassert
{
    public class Expectation<T>
    {
        private Predicate<T> _predicate;
        private Func<T, string> _reasonFalse;
        private Func<T, string> _reasonTrue;

        public Expectation(Predicate<T> predicate, Func<T, string> reasonFalse, Func<T, string> reasonTrue)
        {
            _predicate = predicate;
            _reasonFalse = reasonFalse;
            _reasonTrue = reasonTrue;
        }

        public bool Test(T actual)
        {
            return _predicate(actual);
        }

        public string ReasonFalse(T actual)
        {
            return _reasonFalse(actual);
        }

        public string ReasonTrue(T actual)
        {
            return _reasonTrue(actual);
        }

        public static Expectation<T> operator &(Expectation<T> first, Expectation<T> second)
        {
            return new Expectation<T>(
                actual => first.Test(actual) && second.Test(actual),
                actual => first.Test(actual) ? second.ReasonFalse(actual) : first.ReasonFalse(actual),
                actual => string.Format("{0} and {1}", first.ReasonTrue(actual), second.ReasonTrue(actual))
            );
        }
    }
}
