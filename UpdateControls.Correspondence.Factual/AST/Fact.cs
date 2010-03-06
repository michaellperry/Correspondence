using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.AST
{
    public class Fact
    {
        private int _lineNumber;
        private string _name;
        private List<DataMember> _fields = new List<DataMember>();
        private List<DataMember> _properties = new List<DataMember>();
        private List<Query> _queries = new List<Query>();

        public Fact(string name, int lineNumber)
        {
            _name = name;
            _lineNumber = lineNumber;
        }

        public int LineNumber
        {
            get { return _lineNumber; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<DataMember> Fields
        {
            get { return _fields; }
        }

        public void AddField(DataMember field)
        {
            _fields.Add(field);
        }

        public IEnumerable<DataMember> Properties
        {
            get { return _properties; }
        }

        public void AddProperty(DataMember property)
        {
            _properties.Add(property);
        }

        public IEnumerable<Query> Queries
        {
            get { return _queries; }
        }

        public void AddQuery(Query query)
        {
            _queries.Add(query);
        }
    }
}
