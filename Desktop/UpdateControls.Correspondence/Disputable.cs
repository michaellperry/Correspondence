using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Correspondence
{
    public static class DisputableExtensions
    {
        public static Disputable<T> AsDisputable<T>(this IEnumerable<T> candidates)
        {
            return new Disputable<T>(candidates);
        }
    }
    public class Disputable<T>
    {
        private readonly List<T> _candidates;

        public Disputable(IEnumerable<T> candidates)
        {
            _candidates = candidates.ToList();
        }

        public T Value
        {
            get { return _candidates.FirstOrDefault(); }
        }

        public bool InConflict
        {
            get { return _candidates.Count > 1; }
        }

        public IEnumerable<T> Candidates
        {
            get { return _candidates; }
        }

        public static implicit operator T(Disputable<T> disputable)
        {
            return disputable.Value;
        }

        public static implicit operator Disputable<T>(T value)
        {
            return Enumerable.Repeat(value, 1).AsDisputable();
        }
    }
}
