using System;

namespace UpdateControls.Correspondence.Strategy
{
    public class QueryOptions
    {
        private int _limit;

        public QueryOptions(int limit)
        {
            _limit = limit;
        }

        public int Limit
        {
            get { return _limit; }
        }
    }
}
