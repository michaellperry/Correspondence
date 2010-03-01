using System;

namespace Reversi.GameModel.InternalDSL.Correspondence
{
    public class Predecessor<TFactType>
        where TFactType : CorrespondenceFact
    {
        public bool Is(TFactType fact)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Predecessor<TFactType>(TFactType fact)
        {
            throw new NotImplementedException();
        }

        public static implicit operator TFactType(Predecessor<TFactType> predecessor)
        {
            throw new NotImplementedException();
        }
    }
}
