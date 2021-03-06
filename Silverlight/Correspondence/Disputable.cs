﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Correspondence
{
    public static class DisputableExtensions
    {
        public static Disputable<T> AsDisputable<T>(this IEnumerable<T> candidates)
        {
            return new Disputable<T>(candidates);
        }

        public static Disputable<T> AsDisputable<T>(this IEnumerable<T> candidates, Func<T> getNullInstance)
        {
            return new Disputable<T>(candidates, getNullInstance);
        }
    }

    public class Disputable<T> : IComparable<Disputable<T>>, IComparable
    {
        private readonly List<T> _candidates;
        private readonly Func<T> _getNullInstance;

        public Disputable(IEnumerable<T> candidates)
            : this(candidates, () => default(T))
        {
        }

        public Disputable(IEnumerable<T> candidates, Func<T> getNullInstance)
        {
            _candidates = candidates.ToList();
            _getNullInstance = getNullInstance;
        }

        public T Value
        {
            get { return _candidates.Any() ? _candidates.First() : _getNullInstance(); }
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

        public int CompareTo(Disputable<T> other)
        {
            var strictComparable = Value as IComparable<T>;
            if (strictComparable != null)
                return strictComparable.CompareTo(other.Value);

            var looseComparable = Value as IComparable;
            if (looseComparable != null)
                return looseComparable.CompareTo(other.Value);

            return
                (Value != null ? Value.GetHashCode() : 0) -
                (other.Value != null ? other.Value.GetHashCode() : 0);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Disputable<T>;
            if (other != null)
                return CompareTo(other);
            else
                return -1;
        }
    }
}
