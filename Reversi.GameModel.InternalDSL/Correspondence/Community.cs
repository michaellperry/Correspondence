using System;
using System.Linq.Expressions;

namespace Reversi.GameModel.InternalDSL.Correspondence
{
    public class Community
    {
        public QueryResult<TFactType> Query<TFactType>(Expression<Predicate<TFactType>> predicate)
            where TFactType : CorrespondenceFact
        {
            throw new NotImplementedException();
        }

        public TFactType AddFact<TFactType>(TFactType fact)
            where TFactType : CorrespondenceFact
        {
            throw new NotImplementedException();
        }
    }
}
