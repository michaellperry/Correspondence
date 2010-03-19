using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Class
    {
        private string _name;
        private List<Field> _fields = new List<Field>();
        private List<Predecessor> _predecessors = new List<Predecessor>();
        private List<Query> _queries = new List<Query>();

        public Class(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<Field> Fields
        {
            get { return _fields; }
        }

        public Class AddField(Field field)
        {
            _fields.Add(field);
            return this;
        }

        public IEnumerable<Predecessor> Predecessors
        {
            get { return _predecessors; }
        }

        public Class AddPredecessor(Predecessor predecessor)
        {
            _predecessors.Add(predecessor);
            return this;
        }

        public IEnumerable<Query> Queries
        {
            get { return _queries; }
        }

        public Class AddQuery(Query query)
        {
            _queries.Add(query);
            return this;
        }
    }
}