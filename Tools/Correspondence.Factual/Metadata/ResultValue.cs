using System;

namespace Correspondence.Factual.Metadata
{
    public abstract class ResultValue : Result
    {
        private Cardinality _cardinality;

        public ResultValue(string type, Query query, Cardinality cardinality)
            : base(type, query)
        {
            _cardinality = cardinality;
        }

        public Cardinality Cardinality
        {
            get { return _cardinality; }
        }
    }
}
