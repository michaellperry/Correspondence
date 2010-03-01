using System;
using System.Collections.Generic;

namespace Reversi.GameModel.InternalDSL.Correspondence
{
    public class QueryResult<TFactType> : IEnumerable<TFactType>
    {
        public CorrespondencePredicate DoesNotExist()
        {
            throw new NotImplementedException();
        }

        #region IEnumerable<TFactType> Members

        public IEnumerator<TFactType> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
