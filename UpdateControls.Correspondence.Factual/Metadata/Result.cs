using System;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public abstract class Result
    {
        private string _type;
        private Query _query;

        public Result(string type, Query query)
        {
            _type = type;
            _query = query;
        }

        public string Type
        {
            get { return _type; }
        }

        public Query Query
        {
            get { return _query; }
        }
    }
}
