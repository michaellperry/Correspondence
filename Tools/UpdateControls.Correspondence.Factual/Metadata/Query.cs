using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Query
    {
        private string _name;
        private List<Join> _joins = new List<Join>();

        public Query(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Join> Joins
        {
            get { return _joins; }
        }

        public Query AddJoin(Join join)
        {
            _joins.Add(join);
            return this;
        }
    }
}
