using System;
using System.Collections.Generic;

namespace UpdateControls.Correspondence.Factual.Metadata
{
    public class Class
    {
        private string _name;
        private bool _publicKey;
        private bool _sharedKey;
        private List<Field> _fields = new List<Field>();
        private List<Predecessor> _predecessors = new List<Predecessor>();
        private List<Query> _queries = new List<Query>();
        private List<Result> _results = new List<Result>();
        private List<Predicate> _predicates = new List<Predicate>();

        public Class(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Version { get; set; }

        public bool Unique { get; set; }

        public Path SignedBy { get; set; }

        public Path EncryptedFor { get; set; }

        public Path KeyFor { get; set; }

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

        public IEnumerable<Result> Results
        {
            get { return _results; }
        }

        public Class AddResult(Result result)
        {
            _results.Add(result);
            return this;
        }

        public IEnumerable<Predicate> Predicates
        {
            get { return _predicates; }
        }

        public Class AddPredicate(Predicate predicate)
        {
            _predicates.Add(predicate);
            return this;
        }

        public bool HasPublicKey
        {
            get { return _publicKey; }
            set { _publicKey = value; }
        }

        public bool HasSharedKey
        {
            get { return _sharedKey; }
            set { _sharedKey = value; }
        }
    }
}
