using System;

namespace Predassert
{
    public class Expectation<T>
    {
        private Predicate<T> _predicate;
        private Func<T, string> _message;

        public Expectation(Predicate<T> predicate, Func<T, string> message)
        {
            _predicate = predicate;
            _message = message;
        }

        public bool Test(T actual)
        {
            return _predicate(actual);
        }

        public string Message(T actual)
        {
            return _message(actual);
        }
    }
}
