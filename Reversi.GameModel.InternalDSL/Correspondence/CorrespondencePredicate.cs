using System;

namespace Reversi.GameModel.InternalDSL.Correspondence
{
    public class CorrespondencePredicate
    {
        public CorrespondencePredicate()
        {
        }

        public CorrespondencePredicate And()
        {
            throw new NotImplementedException();
        }

        public static CorrespondencePredicate operator &(CorrespondencePredicate a, CorrespondencePredicate b)
        {
            throw new NotImplementedException();
        }

        public static implicit operator bool(CorrespondencePredicate predicate)
        {
            throw new NotImplementedException();
        }
    }
}
