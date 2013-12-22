using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class ResultValueFact : ResultValue
    {
        private string _factType;

        public ResultValueFact(string type, Query query, Cardinality cardinality, string factType)
            : base(type, query, cardinality)
        {
            _factType = factType;
        }

        public string FactType
        {
            get { return _factType; }
        }
    }
}
