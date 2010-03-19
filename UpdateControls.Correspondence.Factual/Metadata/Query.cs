using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Query
    {
        private string _name;
        private string _type;
        private List<Join> _joins = new List<Join>();

        public Query(string name, string type)
        {
            _name = name;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get { return _type; }
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
