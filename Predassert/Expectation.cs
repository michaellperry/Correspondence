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

        public Expectation<T> And(Expectation<T> that)
        {
            return new Expectation<T>(
                actual => this.Test(actual) && that.Test(actual),
                actual => this.Test(actual) ? that.ReasonFalse(actual) : this.ReasonFalse(actual),
                actual => string.Format("{0} and {1}", this.ReasonTrue(actual), that.ReasonTrue(actual))
            );
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
    }
}
